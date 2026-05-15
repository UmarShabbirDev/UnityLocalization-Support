using System;
using System.Collections.Generic;
using UnityEngine;

namespace UmarDev.Localization
{
    /// <summary>
    /// Singleton manager that handles localization at runtime.
    /// Manages current language, provides translations, and notifies listeners of language changes.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalizationManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("[LocalizationManager]");
                        _instance = go.AddComponent<LocalizationManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("Configuration")]
        [Tooltip("Reference to the main localization data asset")]
        public LocalizationData localizationData;

        [Tooltip("Default language code (e.g., 'en')")]
        public string defaultLanguage = "en";

        [Tooltip("Save language preference to PlayerPrefs")]
        public bool saveLanguagePreference = true;

        [Header("Runtime State")]
        [SerializeField, Tooltip("Current active language code")]
        private string currentLanguage;

        /// <summary>
        /// Event fired when language changes. Passes the new language code.
        /// </summary>
        public event Action<string> OnLanguageChanged;

        /// <summary>
        /// Get the current active language code
        /// </summary>
        public string CurrentLanguage => currentLanguage;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeLanguage();
        }

        /// <summary>
        /// Initialize the language from saved preference or default
        /// </summary>
        private void InitializeLanguage()
        {
            if (localizationData == null)
            {
                Debug.LogWarning("[LocalizationManager] No LocalizationData assigned! Please assign it in the inspector or create one via Tools > UmarDev > Localization Setup");
                return;
            }

            // Load saved language or use default
            if (saveLanguagePreference && PlayerPrefs.HasKey("UmarDev.Localization.Language"))
            {
                currentLanguage = PlayerPrefs.GetString("UmarDev.Localization.Language");
            }
            else
            {
                currentLanguage = defaultLanguage;
            }

            // Validate language exists
            if (!localizationData.HasLanguage(currentLanguage))
            {
                Debug.LogWarning($"[LocalizationManager] Language '{currentLanguage}' not found in data. Using default '{defaultLanguage}'");
                currentLanguage = defaultLanguage;
            }
        }

        /// <summary>
        /// Switch to a different language
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            if (localizationData == null)
            {
                Debug.LogError("[LocalizationManager] Cannot set language - no LocalizationData assigned!");
                return;
            }

            if (!localizationData.HasLanguage(languageCode))
            {
                Debug.LogError($"[LocalizationManager] Language '{languageCode}' not found in data!");
                return;
            }

            currentLanguage = languageCode;

            // Save preference
            if (saveLanguagePreference)
            {
                PlayerPrefs.SetString("UmarDev.Localization.Language", currentLanguage);
                PlayerPrefs.Save();
            }

            // Notify all listeners
            OnLanguageChanged?.Invoke(currentLanguage);
            Debug.Log($"[LocalizationManager] Language changed to: {languageCode}");
        }

        /// <summary>
        /// Get a translated string for the current language
        /// </summary>
        public string GetText(string key)
        {
            if (localizationData == null)
            {
                return $"[NO DATA: {key}]";
            }

            return localizationData.GetTranslation(key, currentLanguage);
        }

        /// <summary>
        /// Get a translated string for a specific language
        /// </summary>
        public string GetText(string key, string languageCode)
        {
            if (localizationData == null)
            {
                return $"[NO DATA: {key}]";
            }

            return localizationData.GetTranslation(key, languageCode);
        }

        /// <summary>
        /// Get the current language data
        /// </summary>
        public LanguageData GetCurrentLanguageData()
        {
            if (localizationData == null) return null;
            return localizationData.GetLanguage(currentLanguage);
        }

        /// <summary>
        /// Get all available languages
        /// </summary>
        public List<LanguageData> GetAvailableLanguages()
        {
            if (localizationData == null) return new List<LanguageData>();
            return localizationData.languages;
        }

        /// <summary>
        /// Check if a specific key exists in the current language
        /// </summary>
        public bool HasKey(string key)
        {
            if (localizationData == null) return false;
            return localizationData.GetAllKeys().Contains(key);
        }
    }
}
