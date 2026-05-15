using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UmarDev.Localization.Editor
{
    public class ImportWindow : EditorWindow
    {
        private LocalizationData targetData;
        private string filePath = "";
        private ImportFormat importFormat = ImportFormat.CSV;
        private bool overwriteExisting = false;
        private Vector2 scrollPosition;
        private string previewText = "";

        private enum ImportFormat
        {
            CSV,
            JSON
        }

        public static void ShowWindow(LocalizationData data)
        {
            var window = GetWindow<ImportWindow>("Import Translations");
            window.minSize = new Vector2(500, 400);
            window.targetData = data;
            window.Show();
        }

        private void OnGUI()
        {
            if (targetData == null)
            {
                EditorGUILayout.HelpBox("No LocalizationData assigned!", MessageType.Error);
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Import Translations", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Target data
            EditorGUILayout.LabelField("Target:", targetData.name);
            GUILayout.Space(10);

            // Import format
            importFormat = (ImportFormat)EditorGUILayout.EnumPopup("Import Format:", importFormat);

            // File path
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File:", GUILayout.Width(40));
            filePath = EditorGUILayout.TextField(filePath);
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                string extension = importFormat == ImportFormat.CSV ? "csv" : "json";
                string newPath = EditorUtility.OpenFilePanel($"Select {extension.ToUpper()} file", LocalizationSettings.LastImportPath, extension);
                if (!string.IsNullOrEmpty(newPath))
                {
                    filePath = newPath;
                    LocalizationSettings.LastImportPath = Path.GetDirectoryName(newPath);
                    LoadPreview();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Options
            overwriteExisting = EditorGUILayout.Toggle("Overwrite Existing", overwriteExisting);

            GUILayout.Space(10);

            // Preview
            if (!string.IsNullOrEmpty(previewText))
            {
                GUILayout.Label("Preview:", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                EditorGUILayout.TextArea(previewText, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }

            GUILayout.Space(10);

            // Import button
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(filePath) || !File.Exists(filePath));
            if (GUILayout.Button("Import", GUILayout.Height(35)))
            {
                PerformImport();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            // Format info
            DrawFormatInfo();
        }

        private void LoadPreview()
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                previewText = "";
                return;
            }

            try
            {
                string content = File.ReadAllText(filePath);
                previewText = content.Length > 1000 ? content.Substring(0, 1000) + "\n..." : content;
            }
            catch (Exception e)
            {
                previewText = $"Error reading file: {e.Message}";
            }
        }

        private void PerformImport()
        {
            if (!File.Exists(filePath))
            {
                EditorUtility.DisplayDialog("Error", "File not found!", "OK");
                return;
            }

            // Create backup if enabled
            if (LocalizationSettings.AutoBackup)
            {
                CreateBackup();
            }

            try
            {
                if (importFormat == ImportFormat.CSV)
                {
                    ImportCSV();
                }
                else
                {
                    ImportJSON();
                }

                EditorUtility.SetDirty(targetData);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Success", "Import completed successfully!", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import Failed", $"Error: {e.Message}", "OK");
                Debug.LogError($"[Import] Failed: {e}");
            }
        }

        private void ImportCSV()
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 2)
            {
                throw new Exception("CSV file must have at least a header row and one data row");
            }

            // Parse header (first row is keys, first column is "key", rest are language codes)
            string[] headers = ParseCSVLine(lines[0]);
            if (headers.Length < 2 || headers[0].ToLower() != "key")
            {
                throw new Exception("CSV must have 'key' as first column");
            }

            // Get language codes from header
            List<string> languageCodes = new List<string>();
            for (int i = 1; i < headers.Length; i++)
            {
                languageCodes.Add(headers[i]);
            }

            // Ensure languages exist in data
            foreach (var langCode in languageCodes)
            {
                if (!targetData.HasLanguage(langCode))
                {
                    targetData.languages.Add(new LanguageData
                    {
                        code = langCode,
                        displayName = langCode.ToUpper(),
                        isRTL = false
                    });
                }
            }

            // Parse data rows
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = ParseCSVLine(lines[i]);
                if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

                string key = values[0];

                // Skip if not overwriting and key exists
                if (!overwriteExisting && targetData.translations.Exists(t => t.key == key))
                {
                    continue;
                }

                // Set translations for each language
                for (int j = 1; j < values.Length && j < headers.Length; j++)
                {
                    string langCode = languageCodes[j - 1];
                    string text = values[j];
                    targetData.SetTranslation(key, langCode, text);
                }
            }

            Debug.Log($"[Import] Imported {lines.Length - 1} entries from CSV");
        }

        private void ImportJSON()
        {
            string json = File.ReadAllText(filePath);
            var import = JsonUtility.FromJson<LocalizationImportData>(json);

            if (import == null)
            {
                throw new Exception("Failed to parse JSON");
            }

            // Import languages
            foreach (var lang in import.languages)
            {
                if (!targetData.HasLanguage(lang.code))
                {
                    targetData.languages.Add(lang);
                }
            }

            // Import translations
            int count = 0;
            foreach (var entry in import.translations)
            {
                if (!overwriteExisting && targetData.translations.Exists(t => t.key == entry.key))
                {
                    continue;
                }

                foreach (var translation in entry.values)
                {
                    targetData.SetTranslation(entry.key, translation.languageCode, translation.text);
                    count++;
                }
            }

            Debug.Log($"[Import] Imported {count} translations from JSON");
        }

        private string[] ParseCSVLine(string line)
        {
            List<string> values = new List<string>();
            bool inQuotes = false;
            StringBuilder current = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            values.Add(current.ToString().Trim());
            return values.ToArray();
        }

        private void CreateBackup()
        {
            try
            {
                string assetPath = AssetDatabase.GetAssetPath(targetData);
                string directory = Path.GetDirectoryName(assetPath);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupPath = Path.Combine(directory, $"{fileName}_backup_{timestamp}.asset");

                AssetDatabase.CopyAsset(assetPath, backupPath);
                Debug.Log($"[Import] Created backup at {backupPath}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Import] Failed to create backup: {e.Message}");
            }
        }

        private void DrawFormatInfo()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Format Information", EditorStyles.boldLabel);

            if (importFormat == ImportFormat.CSV)
            {
                GUILayout.Label("CSV Format:", EditorStyles.boldLabel);
                GUILayout.Label("First row: key,en,es,fr,...", EditorStyles.wordWrappedMiniLabel);
                GUILayout.Label("Data rows: ui.button.start,Start,Iniciar,Commencer,...", EditorStyles.wordWrappedMiniLabel);
                GUILayout.Label("", EditorStyles.wordWrappedMiniLabel);
                GUILayout.Label("Example:", EditorStyles.boldLabel);
                GUILayout.Label("key,en,es\nui.welcome,Welcome,Bienvenido\nui.exit,Exit,Salir", EditorStyles.wordWrappedMiniLabel);
            }
            else
            {
                GUILayout.Label("JSON Format: Standard LocalizationData JSON export", EditorStyles.wordWrappedMiniLabel);
            }

            EditorGUILayout.EndVertical();
        }

        [Serializable]
        private class LocalizationImportData
        {
            public List<LanguageData> languages = new List<LanguageData>();
            public List<TranslationEntry> translations = new List<TranslationEntry>();
        }
    }
}
