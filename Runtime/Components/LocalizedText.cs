using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

namespace UmarDev.Localization
{
    /// <summary>
    /// Component that automatically updates Text or TextMeshPro components
    /// with localized text based on a translation key.
    /// </summary>
    [DisallowMultipleComponent]
    public class LocalizedText : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The translation key to use (e.g., 'ui.button.start')")]
        public string translationKey;

        [Tooltip("Use TextMeshPro instead of legacy Text component")]
        public bool useTextMeshPro = true;

        [Header("References")]
        [SerializeField, Tooltip("Cached Text component (legacy UI)")]
        private Text textComponent;

#if TMP_PRESENT
        [SerializeField, Tooltip("Cached TextMeshPro component")]
        private TextMeshProUGUI tmpComponent;
#endif

        private void Awake()
        {
            // Auto-detect components if not assigned
            if (textComponent == null)
            {
                textComponent = GetComponent<Text>();
            }

#if TMP_PRESENT
            if (tmpComponent == null)
            {
                tmpComponent = GetComponent<TextMeshProUGUI>();
            }

            // Auto-detect which type to use
            if (tmpComponent != null)
            {
                useTextMeshPro = true;
            }
            else
#endif
            if (textComponent != null)
            {
                useTextMeshPro = false;
            }
        }

        private void OnEnable()
        {
            // Subscribe to language change events
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
            }

            // Update text immediately
            UpdateText();
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        /// <summary>
        /// Called when the language changes
        /// </summary>
        private void OnLanguageChanged(string newLanguage)
        {
            UpdateText();
            UpdateFont();
        }

        /// <summary>
        /// Update the text with the current translation
        /// </summary>
        public void UpdateText()
        {
            if (string.IsNullOrEmpty(translationKey))
            {
                Debug.LogWarning($"[LocalizedText] No translation key set on {gameObject.name}", this);
                return;
            }

            if (LocalizationManager.Instance == null)
            {
                Debug.LogWarning($"[LocalizedText] LocalizationManager not found! Make sure it's in the scene.", this);
                return;
            }

            string translatedText = LocalizationManager.Instance.GetText(translationKey);

            // Apply to the appropriate component
#if TMP_PRESENT
            if (useTextMeshPro && tmpComponent != null)
            {
                tmpComponent.text = translatedText;
            }
            else
#endif
            if (textComponent != null)
            {
                textComponent.text = translatedText;
            }
            else
            {
                Debug.LogWarning($"[LocalizedText] No Text or TextMeshPro component found on {gameObject.name}", this);
            }
        }

        /// <summary>
        /// Update font based on current language (if language has custom font)
        /// </summary>
        private void UpdateFont()
        {
            if (LocalizationManager.Instance == null) return;

            var languageData = LocalizationManager.Instance.GetCurrentLanguageData();
            if (languageData == null) return;

#if TMP_PRESENT
            // Apply TMP font if available
            if (useTextMeshPro && tmpComponent != null && languageData.tmpFont != null)
            {
                tmpComponent.font = languageData.tmpFont;
            }
            // Handle RTL (Right-to-Left) languages
            else if (useTextMeshPro && tmpComponent != null)
            {
                tmpComponent.isRightToLeftText = languageData.isRTL;
            }
            else
#endif
            // Apply legacy font if available
            if (textComponent != null && languageData.font != null)
            {
                textComponent.font = languageData.font;
            }
        }

        /// <summary>
        /// Change the translation key at runtime
        /// </summary>
        public void SetKey(string newKey)
        {
            translationKey = newKey;
            UpdateText();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor helper to update text in edit mode
        /// </summary>
        private void OnValidate()
        {
            // Auto-detect components
            if (textComponent == null)
            {
                textComponent = GetComponent<Text>();
            }

#if TMP_PRESENT
            if (tmpComponent == null)
            {
                tmpComponent = GetComponent<TextMeshProUGUI>();
            }
#endif
        }
#endif
    }
}
