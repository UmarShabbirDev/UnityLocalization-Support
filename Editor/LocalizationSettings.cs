using UnityEditor;
using UnityEngine;

namespace UmarDev.Localization.Editor
{
    public static class LocalizationSettings
    {
        private const string PREFIX = "UmarDev.Localization.";

        // Keys
        private const string KEY_DATA_PATH = PREFIX + "DataPath";
        private const string KEY_LAST_IMPORT_PATH = PREFIX + "LastImportPath";
        private const string KEY_AUTO_BACKUP = PREFIX + "AutoBackup";
        private const string KEY_SHOW_WARNINGS = PREFIX + "ShowWarnings";

        public static string LocalizationDataPath
        {
            get => EditorPrefs.GetString(KEY_DATA_PATH, "Assets/Resources/LocalizationData.asset");
            set => EditorPrefs.SetString(KEY_DATA_PATH, value);
        }

        public static string LastImportPath
        {
            get => EditorPrefs.GetString(KEY_LAST_IMPORT_PATH, "");
            set => EditorPrefs.SetString(KEY_LAST_IMPORT_PATH, value);
        }


        public static bool AutoBackup
        {
            get => EditorPrefs.GetBool(KEY_AUTO_BACKUP, true);
            set => EditorPrefs.SetBool(KEY_AUTO_BACKUP, value);
        }

        public static bool ShowWarnings
        {
            get => EditorPrefs.GetBool(KEY_SHOW_WARNINGS, true);
            set => EditorPrefs.SetBool(KEY_SHOW_WARNINGS, value);
        }

        public static LocalizationData LoadLocalizationData()
        {
            string path = LocalizationDataPath;
            var data = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);

            if (data == null && ShowWarnings)
            {
                Debug.LogWarning($"[LocalizationSettings] No LocalizationData found at '{path}'. Create one via Tools > UmarDev > Localization Setup");
            }

            return data;
        }

        public static void SaveLocalizationDataPath(LocalizationData data)
        {
            if (data == null) return;

            string path = AssetDatabase.GetAssetPath(data);
            if (!string.IsNullOrEmpty(path))
            {
                LocalizationDataPath = path;
            }
        }

        public static void ResetToDefaults()
        {
            EditorPrefs.DeleteKey(KEY_DATA_PATH);
            EditorPrefs.DeleteKey(KEY_LAST_IMPORT_PATH);
            EditorPrefs.DeleteKey(KEY_AUTO_BACKUP);
            EditorPrefs.DeleteKey(KEY_SHOW_WARNINGS);

            Debug.Log("[LocalizationSettings] Settings reset to defaults");
        }
    }
}
