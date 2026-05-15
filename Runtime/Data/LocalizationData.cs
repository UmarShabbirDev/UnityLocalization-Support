using System;
using System.Collections.Generic;
using UnityEngine;

namespace UmarDev.Localization
{
    /// <summary>
    /// ScriptableObject that stores all localization data for the project.
    /// This is the main data container for translations.
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "UmarDev/Localization Data", order = 1)]
    public class LocalizationData : ScriptableObject
    {
        [Tooltip("List of supported languages")]
        public List<LanguageData> languages = new List<LanguageData>();

        [Tooltip("All translation entries")]
        public List<TranslationEntry> translations = new List<TranslationEntry>();

        /// <summary>
        /// Get translation for a specific key and language code
        /// </summary>
        public string GetTranslation(string key, string languageCode)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            var entry = translations.Find(t => t.key == key);
            if (entry == null) return $"[{key}]"; // Return key in brackets if not found

            var translation = entry.values.Find(v => v.languageCode == languageCode);
            return translation?.text ?? $"[{key}]";
        }

        /// <summary>
        /// Check if a language code exists
        /// </summary>
        public bool HasLanguage(string languageCode)
        {
            return languages.Exists(l => l.code == languageCode);
        }

        /// <summary>
        /// Get language data by code
        /// </summary>
        public LanguageData GetLanguage(string languageCode)
        {
            return languages.Find(l => l.code == languageCode);
        }

        /// <summary>
        /// Add or update a translation entry
        /// </summary>
        public void SetTranslation(string key, string languageCode, string text)
        {
            var entry = translations.Find(t => t.key == key);
            if (entry == null)
            {
                entry = new TranslationEntry { key = key };
                translations.Add(entry);
            }

            var translation = entry.values.Find(v => v.languageCode == languageCode);
            if (translation == null)
            {
                entry.values.Add(new Translation { languageCode = languageCode, text = text });
            }
            else
            {
                translation.text = text;
            }
        }

        /// <summary>
        /// Get all translation keys
        /// </summary>
        public List<string> GetAllKeys()
        {
            var keys = new List<string>();
            foreach (var entry in translations)
            {
                keys.Add(entry.key);
            }
            return keys;
        }
    }

    /// <summary>
    /// Represents a language with metadata
    /// </summary>
    [Serializable]
    public class LanguageData
    {
        [Tooltip("Language code (e.g., 'en', 'es', 'fr', 'ar')")]
        public string code;

        [Tooltip("Display name (e.g., 'English', 'Español', 'Français')")]
        public string displayName;

        [Tooltip("Optional font to use for this language")]
        public Font font;

        [Tooltip("Optional TMP font asset for this language")]
        public TMPro.TMP_FontAsset tmpFont;

        [Tooltip("Is this a right-to-left language?")]
        public bool isRTL;
    }

    /// <summary>
    /// A single translation entry with all language variants
    /// </summary>
    [Serializable]
    public class TranslationEntry
    {
        [Tooltip("Unique key for this translation (e.g., 'ui.button.start')")]
        public string key;

        [Tooltip("All translations for different languages")]
        public List<Translation> values = new List<Translation>();
    }

    /// <summary>
    /// A translation for a specific language
    /// </summary>
    [Serializable]
    public class Translation
    {
        [Tooltip("Language code")]
        public string languageCode;

        [Tooltip("Translated text")]
        [TextArea(2, 5)]
        public string text;
    }
}
