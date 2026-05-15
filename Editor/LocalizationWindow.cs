using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UmarDev.Localization.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        private LocalizationData localizationData;
        private Vector2 scrollPosition;
        private string searchFilter = "";
        private int selectedLanguageIndex = 0;
        private bool showSettings = false;

        // Add new entry fields
        private string newKey = "";
        private bool showAddEntry = false;

        [MenuItem("Window/UmarDev/Localization Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationWindow>("Localization");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        private void OnEnable()
        {
            LoadData();
        }

        private void LoadData()
        {
            localizationData = LocalizationSettings.LoadLocalizationData();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (localizationData == null)
            {
                DrawNoDataScreen();
                return;
            }

            if (showSettings)
            {
                DrawSettingsPanel();
            }
            else
            {
                DrawMainContent();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // Data reference
            EditorGUI.BeginChangeCheck();
            localizationData = (LocalizationData)EditorGUILayout.ObjectField(
                localizationData,
                typeof(LocalizationData),
                false,
                GUILayout.Width(200)
            );
            if (EditorGUI.EndChangeCheck() && localizationData != null)
            {
                LocalizationSettings.SaveLocalizationDataPath(localizationData);
            }

            GUILayout.FlexibleSpace();

            // Import button
            if (GUILayout.Button("Import CSV/JSON", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                ImportWindow.ShowWindow(localizationData);
            }

            // Settings toggle
            showSettings = GUILayout.Toggle(showSettings, "Settings", EditorStyles.toolbarButton, GUILayout.Width(70));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoDataScreen()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();

            GUILayout.Label("No Localization Data Found", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("Create a LocalizationData asset to get started.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(20);

            if (GUILayout.Button("Create LocalizationData Asset", GUILayout.Height(30)))
            {
                CreateLocalizationData();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Run Setup Wizard", GUILayout.Height(30)))
            {
                LocalizationSetupWizard.ShowWindow();
            }

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        private void CreateLocalizationData()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create LocalizationData",
                "LocalizationData",
                "asset",
                "Choose where to save the LocalizationData asset"
            );

            if (!string.IsNullOrEmpty(path))
            {
                var data = CreateInstance<LocalizationData>();

                // Add default English language
                data.languages.Add(new LanguageData
                {
                    code = "en",
                    displayName = "English",
                    isRTL = false
                });

                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                localizationData = data;
                LocalizationSettings.SaveLocalizationDataPath(data);

                Debug.Log($"[Localization] Created LocalizationData at {path}");
            }
        }

        private void DrawSettingsPanel()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(10);
            GUILayout.Label("Localization Settings", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Auto Backup
            LocalizationSettings.AutoBackup = EditorGUILayout.Toggle(
                new GUIContent("Auto Backup", "Automatically create backups before importing data"),
                LocalizationSettings.AutoBackup
            );

            // Show Warnings
            LocalizationSettings.ShowWarnings = EditorGUILayout.Toggle(
                new GUIContent("Show Warnings", "Show warnings for missing translations"),
                LocalizationSettings.ShowWarnings
            );

            GUILayout.Space(20);

            // Data path info
            EditorGUILayout.LabelField("Data Path:", LocalizationSettings.LocalizationDataPath, EditorStyles.wordWrappedLabel);

            GUILayout.Space(20);

            // Actions
            if (GUILayout.Button("Reset Settings to Default", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset all settings to default?", "Yes", "Cancel"))
                {
                    LocalizationSettings.ResetToDefaults();
                }
            }

            if (GUILayout.Button("Open Setup Wizard", GUILayout.Height(25)))
            {
                LocalizationSetupWizard.ShowWindow();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawMainContent()
        {
            if (localizationData.languages.Count == 0)
            {
                EditorGUILayout.HelpBox("No languages configured. Add languages below to get started.", MessageType.Info);
            }

            // Language management section
            DrawLanguageSection();

            GUILayout.Space(10);

            // Translation entries section
            DrawTranslationsSection();
        }

        private void DrawLanguageSection()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Languages", EditorStyles.boldLabel);

            // Display existing languages
            for (int i = 0; i < localizationData.languages.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var lang = localizationData.languages[i];

                EditorGUI.BeginChangeCheck();
                lang.code = EditorGUILayout.TextField(lang.code, GUILayout.Width(50));
                lang.displayName = EditorGUILayout.TextField(lang.displayName, GUILayout.Width(150));
                lang.isRTL = EditorGUILayout.Toggle("RTL", lang.isRTL, GUILayout.Width(50));

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(localizationData);
                }

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    if (EditorUtility.DisplayDialog("Remove Language",
                        $"Remove language '{lang.displayName}'? This will delete all translations for this language.",
                        "Remove", "Cancel"))
                    {
                        localizationData.languages.RemoveAt(i);
                        EditorUtility.SetDirty(localizationData);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // Add new language button
            if (GUILayout.Button("+ Add Language"))
            {
                localizationData.languages.Add(new LanguageData
                {
                    code = "new",
                    displayName = "New Language",
                    isRTL = false
                });
                EditorUtility.SetDirty(localizationData);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTranslationsSection()
        {
            EditorGUILayout.BeginVertical("box");

            // Header
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Translations", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // Search
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchFilter = EditorGUILayout.TextField(searchFilter, GUILayout.Width(200));

            // Add entry toggle
            showAddEntry = GUILayout.Toggle(showAddEntry, "+ Add Entry", "button", GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();

            // Add new entry panel
            if (showAddEntry)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Add New Translation Entry", EditorStyles.boldLabel);

                newKey = EditorGUILayout.TextField("Key:", newKey);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Entry"))
                {
                    if (!string.IsNullOrEmpty(newKey))
                    {
                        if (localizationData.translations.Exists(t => t.key == newKey))
                        {
                            EditorUtility.DisplayDialog("Key Exists", $"A translation with key '{newKey}' already exists.", "OK");
                        }
                        else
                        {
                            var entry = new TranslationEntry { key = newKey };
                            localizationData.translations.Add(entry);
                            EditorUtility.SetDirty(localizationData);
                            newKey = "";
                            showAddEntry = false;
                        }
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    newKey = "";
                    showAddEntry = false;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);

            // Translations list
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (localizationData.translations.Count == 0)
            {
                GUILayout.Label("No translations yet. Click '+ Add Entry' to create one.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                DrawTranslationEntries();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawTranslationEntries()
        {
            for (int i = localizationData.translations.Count - 1; i >= 0; i--)
            {
                var entry = localizationData.translations[i];

                // Apply search filter
                if (!string.IsNullOrEmpty(searchFilter) && !entry.key.ToLower().Contains(searchFilter.ToLower()))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical("box");

                // Key header with delete button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entry.key, EditorStyles.boldLabel);
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Delete Entry", $"Delete translation key '{entry.key}'?", "Delete", "Cancel"))
                    {
                        localizationData.translations.RemoveAt(i);
                        EditorUtility.SetDirty(localizationData);
                        continue;
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Draw translations for each language
                foreach (var lang in localizationData.languages)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(lang.displayName, GUILayout.Width(100));

                    var translation = entry.values.Find(v => v.languageCode == lang.code);
                    string currentText = translation?.text ?? "";

                    EditorGUI.BeginChangeCheck();
                    string newText = EditorGUILayout.TextArea(currentText, GUILayout.MinHeight(40));

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (translation == null)
                        {
                            entry.values.Add(new Translation { languageCode = lang.code, text = newText });
                        }
                        else
                        {
                            translation.text = newText;
                        }
                        EditorUtility.SetDirty(localizationData);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }
    }
}
