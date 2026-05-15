using System.IO;
using UnityEditor;
using UnityEngine;

namespace UmarDev.Localization.Editor
{
    public class LocalizationSetupWizard : EditorWindow
    {
        private int currentStep = 0;
        private LocalizationData localizationData;
        private GameObject managerObject;
        private string[] languageCodes = new string[] { "en" };
        private string[] languageNames = new string[] { "English" };
        private Vector2 scrollPosition;

        private readonly string[] stepTitles = new string[]
        {
            "Welcome",
            "Create Localization Data",
            "Add Languages",
            "Setup Scene Manager",
            "Complete"
        };

        [MenuItem("Tools/UmarDev/Localization Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationSetupWizard>("Localization Setup");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }

        private void OnGUI()
        {
            DrawHeader();
            GUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            switch (currentStep)
            {
                case 0:
                    DrawWelcomeStep();
                    break;
                case 1:
                    DrawCreateDataStep();
                    break;
                case 2:
                    DrawAddLanguagesStep();
                    break;
                case 3:
                    DrawSetupManagerStep();
                    break;
                case 4:
                    DrawCompleteStep();
                    break;
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            DrawNavigation();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("UmarDev Localization Setup", EditorStyles.boldLabel);
            GUILayout.Label($"Step {currentStep + 1} of {stepTitles.Length}: {stepTitles[currentStep]}", EditorStyles.miniLabel);

            // Progress bar
            Rect rect = GUILayoutUtility.GetRect(18, 18, GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(rect, (float)currentStep / (stepTitles.Length - 1), "");

            EditorGUILayout.EndVertical();
        }

        private void DrawWelcomeStep()
        {
            GUILayout.Label("Welcome to UmarDev Localization Pro!", EditorStyles.largeLabel);
            GUILayout.Space(20);

            GUILayout.Label("This wizard will help you set up the localization system in your project.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            GUILayout.Label("What you'll do:", EditorStyles.boldLabel);
            GUILayout.Label("1. Create a LocalizationData asset to store translations", EditorStyles.wordWrappedLabel);
            GUILayout.Label("2. Add languages to support", EditorStyles.wordWrappedLabel);
            GUILayout.Label("3. Set up the LocalizationManager in your scene", EditorStyles.wordWrappedLabel);
            GUILayout.Label("4. Start adding translations!", EditorStyles.wordWrappedLabel);

            GUILayout.Space(20);
            EditorGUILayout.HelpBox("This wizard is optional. You can also set up manually via Window > UmarDev > Localization Manager", MessageType.Info);
        }

        private void DrawCreateDataStep()
        {
            GUILayout.Label("Create Localization Data Asset", EditorStyles.largeLabel);
            GUILayout.Space(10);

            GUILayout.Label("The LocalizationData asset stores all your translations.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            // Show existing data if found
            localizationData = (LocalizationData)EditorGUILayout.ObjectField(
                "Localization Data:",
                localizationData,
                typeof(LocalizationData),
                false
            );

            GUILayout.Space(10);

            if (localizationData == null)
            {
                EditorGUILayout.HelpBox("No LocalizationData asset found. Create one below.", MessageType.Warning);

                if (GUILayout.Button("Create LocalizationData Asset", GUILayout.Height(35)))
                {
                    CreateLocalizationDataAsset();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("LocalizationData found! Click Next to continue.", MessageType.Info);
            }
        }

        private void CreateLocalizationDataAsset()
        {
            // Create Resources folder if it doesn't exist
            string resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            string path = $"{resourcesPath}/LocalizationData.asset";

            // Check if file already exists
            if (File.Exists(path))
            {
                localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
                EditorUtility.DisplayDialog("Already Exists", "LocalizationData already exists at Assets/Resources/LocalizationData.asset", "OK");
                return;
            }

            // Create the asset
            localizationData = CreateInstance<LocalizationData>();
            AssetDatabase.CreateAsset(localizationData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            LocalizationSettings.SaveLocalizationDataPath(localizationData);

            EditorGUIUtility.PingObject(localizationData);
            Debug.Log($"[Setup] Created LocalizationData at {path}");
        }

        private void DrawAddLanguagesStep()
        {
            GUILayout.Label("Add Languages", EditorStyles.largeLabel);
            GUILayout.Space(10);

            if (localizationData == null)
            {
                EditorGUILayout.HelpBox("Please create LocalizationData in the previous step first.", MessageType.Error);
                return;
            }

            GUILayout.Label("Add the languages you want to support:", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            // Show existing languages in data
            if (localizationData.languages.Count > 0)
            {
                EditorGUILayout.LabelField("Current Languages:", EditorStyles.boldLabel);
                foreach (var lang in localizationData.languages)
                {
                    EditorGUILayout.LabelField($"  {lang.code} - {lang.displayName}");
                }
                GUILayout.Space(10);
            }

            // Quick add common languages
            GUILayout.Label("Quick Add Common Languages:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("English (en)"))
            {
                AddLanguageIfNotExists("en", "English", false);
            }
            if (GUILayout.Button("Spanish (es)"))
            {
                AddLanguageIfNotExists("es", "Español", false);
            }
            if (GUILayout.Button("French (fr)"))
            {
                AddLanguageIfNotExists("fr", "Français", false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("German (de)"))
            {
                AddLanguageIfNotExists("de", "Deutsch", false);
            }
            if (GUILayout.Button("Japanese (ja)"))
            {
                AddLanguageIfNotExists("ja", "日本語", false);
            }
            if (GUILayout.Button("Arabic (ar)"))
            {
                AddLanguageIfNotExists("ar", "العربية", true); // RTL
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Chinese (zh)"))
            {
                AddLanguageIfNotExists("zh", "中文", false);
            }
            if (GUILayout.Button("Russian (ru)"))
            {
                AddLanguageIfNotExists("ru", "Русский", false);
            }
            if (GUILayout.Button("Portuguese (pt)"))
            {
                AddLanguageIfNotExists("pt", "Português", false);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("You can add more languages later via Window > UmarDev > Localization Manager", MessageType.Info);
        }

        private void AddLanguageIfNotExists(string code, string displayName, bool isRTL)
        {
            if (localizationData == null) return;

            if (localizationData.HasLanguage(code))
            {
                EditorUtility.DisplayDialog("Already Exists", $"Language '{code}' already exists.", "OK");
                return;
            }

            localizationData.languages.Add(new LanguageData
            {
                code = code,
                displayName = displayName,
                isRTL = isRTL
            });

            EditorUtility.SetDirty(localizationData);
            AssetDatabase.SaveAssets();

            Debug.Log($"[Setup] Added language: {displayName} ({code})");
        }

        private void DrawSetupManagerStep()
        {
            GUILayout.Label("Setup Scene Manager", EditorStyles.largeLabel);
            GUILayout.Space(10);

            GUILayout.Label("The LocalizationManager handles language switching at runtime.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            // Check if manager already exists
            var existingManager = FindObjectOfType<LocalizationManager>();

            if (existingManager != null)
            {
                EditorGUILayout.HelpBox("LocalizationManager already exists in the scene!", MessageType.Info);
                managerObject = existingManager.gameObject;

                EditorGUILayout.ObjectField("Manager:", managerObject, typeof(GameObject), true);

                GUILayout.Space(10);

                if (GUILayout.Button("Select in Hierarchy"))
                {
                    Selection.activeGameObject = managerObject;
                    EditorGUIUtility.PingObject(managerObject);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No LocalizationManager found in the scene. Create one below.", MessageType.Warning);

                if (GUILayout.Button("Create LocalizationManager", GUILayout.Height(35)))
                {
                    CreateManagerInScene();
                }
            }
        }

        private void CreateManagerInScene()
        {
            managerObject = new GameObject("[LocalizationManager]");
            var manager = managerObject.AddComponent<LocalizationManager>();

            // Assign the localization data
            if (localizationData != null)
            {
                manager.localizationData = localizationData;

                // Set default language to first available language
                if (localizationData.languages.Count > 0)
                {
                    manager.defaultLanguage = localizationData.languages[0].code;
                }
            }

            // Mark as dirty
            Undo.RegisterCreatedObjectUndo(managerObject, "Create LocalizationManager");
            Selection.activeGameObject = managerObject;
            EditorGUIUtility.PingObject(managerObject);

            Debug.Log("[Setup] Created LocalizationManager in scene");
        }

        private void DrawCompleteStep()
        {
            GUILayout.Label("Setup Complete!", EditorStyles.largeLabel);
            GUILayout.Space(20);

            GUILayout.Label("Your localization system is ready to use!", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            GUILayout.Label("Next Steps:", EditorStyles.boldLabel);
            GUILayout.Label("1. Add translations via Window > UmarDev > Localization Manager", EditorStyles.wordWrappedLabel);
            GUILayout.Label("2. Add LocalizedText components to your UI Text elements", EditorStyles.wordWrappedLabel);
            GUILayout.Label("3. Test language switching at runtime", EditorStyles.wordWrappedLabel);

            GUILayout.Space(20);

            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);

            if (GUILayout.Button("Open Localization Manager", GUILayout.Height(30)))
            {
                LocalizationWindow.ShowWindow();
            }

            if (localizationData != null && GUILayout.Button("Select LocalizationData Asset", GUILayout.Height(30)))
            {
                Selection.activeObject = localizationData;
                EditorGUIUtility.PingObject(localizationData);
            }

            if (managerObject != null && GUILayout.Button("Select Manager in Scene", GUILayout.Height(30)))
            {
                Selection.activeGameObject = managerObject;
                EditorGUIUtility.PingObject(managerObject);
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            EditorGUILayout.HelpBox("Check the README for detailed documentation and examples!", MessageType.Info);
        }

        private void DrawNavigation()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(currentStep == 0);
            if (GUILayout.Button("< Previous", GUILayout.Height(30), GUILayout.Width(100)))
            {
                currentStep--;
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();

            if (currentStep < stepTitles.Length - 1)
            {
                // Disable Next button if requirements not met
                bool canProceed = CanProceedToNextStep();
                EditorGUI.BeginDisabledGroup(!canProceed);
                if (GUILayout.Button("Next >", GUILayout.Height(30), GUILayout.Width(100)))
                {
                    currentStep++;
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (GUILayout.Button("Finish", GUILayout.Height(30), GUILayout.Width(100)))
                {
                    Close();
                    LocalizationWindow.ShowWindow();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool CanProceedToNextStep()
        {
            switch (currentStep)
            {
                case 1: // Create Data step
                    return localizationData != null;
                case 2: // Add Languages step
                    return localizationData != null && localizationData.languages.Count > 0;
                default:
                    return true;
            }
        }
    }
}
