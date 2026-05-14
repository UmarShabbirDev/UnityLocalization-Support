# UmarDev Localization Pro

A simple yet powerful localization system for Unity that supports multiple languages, CSV/JSON import, runtime language switching, and automatic text component replacement.

## Features

- **Easy Setup** - Setup wizard guides you through initial configuration
- **Multiple Languages** - Support unlimited languages with RTL text support
- **CSV/JSON Import** - Import translations from spreadsheets or JSON files
- **Runtime Switching** - Change language on the fly without reloading scenes
- **Auto Text Update** - LocalizedText component automatically updates UI when language changes
- **Font Support** - Custom fonts per language (includes TextMeshPro support)
- **Editor Tools** - Powerful editor window for managing all translations
- **Language Persistence** - Automatically saves user's language preference
- **Search & Filter** - Quickly find translations in the editor
- **Backup System** - Auto-backup before imports to prevent data loss

## Installation

### Method 1: Unity Package Manager (Git URL)

1. Open Unity Package Manager (Window > Package Manager)
2. Click the `+` button and select "Add package from git URL"
3. Enter: `https://github.com/UmarShabbirDev/UnityLocalization-Support.git`
4. Click "Add"

### Method 2: Manual Installation

1. Download or clone this repository
2. Copy the `com.umardev.localization` folder into your Unity project's `Packages` folder
3. Unity will automatically detect and import the package

## Quick Start

### Step 1: Run Setup Wizard

1. Go to `Tools > UmarDev > Localization Setup Wizard`
2. Follow the wizard steps:
   - Create LocalizationData asset
   - Add languages (English, Spanish, French, etc.)
   - Create LocalizationManager in your scene
3. Click "Finish"

### Step 2: Add Translations

1. Open `Window > UmarDev > Localization Manager`
2. Click "+ Add Entry" to create a new translation
3. Enter a key (e.g., `ui.button.start`)
4. Fill in translations for each language
5. Save

### Step 3: Localize UI Text

1. Select a Text or TextMeshProUGUI component in your scene
2. Add the `LocalizedText` component
3. Enter your translation key (e.g., `ui.button.start`)
4. The text will automatically update based on the current language!

### Step 4: Switch Languages at Runtime

```csharp
using UmarDev.Localization;

// Switch to Spanish
LocalizationManager.Instance.SetLanguage("es");

// Get current language
string currentLang = LocalizationManager.Instance.CurrentLanguage;

// Get a translation directly
string text = LocalizationManager.Instance.GetText("ui.button.start");
```

## Usage Examples

### Localizing Text Components

```csharp
using UnityEngine;
using UmarDev.Localization;

public class MenuManager : MonoBehaviour
{
    private LocalizedText titleText;

    void Start()
    {
        titleText = GetComponent<LocalizedText>();

        // Change translation key dynamically
        titleText.SetKey("ui.title.welcome");
    }
}
```

### Language Selector Dropdown

```csharp
using UnityEngine;
using UnityEngine.UI;
using UmarDev.Localization;

public class LanguageSelector : MonoBehaviour
{
    public Dropdown languageDropdown;

    void Start()
    {
        // Populate dropdown with available languages
        languageDropdown.ClearOptions();

        var languages = LocalizationManager.Instance.GetAvailableLanguages();
        var options = new System.Collections.Generic.List<string>();

        foreach (var lang in languages)
        {
            options.Add(lang.displayName);
        }

        languageDropdown.AddOptions(options);

        // Listen for changes
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    void OnLanguageChanged(int index)
    {
        var languages = LocalizationManager.Instance.GetAvailableLanguages();
        LocalizationManager.Instance.SetLanguage(languages[index].code);
    }
}
```

### Listen to Language Changes

```csharp
using UnityEngine;
using UmarDev.Localization;

public class CustomLocalizer : MonoBehaviour
{
    void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    void OnDisable()
    {
        LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
    }

    void OnLanguageChanged(string newLanguageCode)
    {
        Debug.Log($"Language changed to: {newLanguageCode}");
        // Update your custom UI elements here
    }
}
```

## CSV Import Format

Create a CSV file with this format:

```csv
key,en,es,fr
ui.button.start,Start,Iniciar,Commencer
ui.button.exit,Exit,Salir,Quitter
ui.welcome,Welcome to the game!,¡Bienvenido al juego!,Bienvenue dans le jeu!
ui.loading,Loading...,Cargando...,Chargement...
```

- First column must be "key"
- Other columns are language codes (en, es, fr, etc.)
- Use quotes for text with commas: `"Hello, world!"`

Import via `Window > UmarDev > Localization Manager > Import CSV/JSON`

## JSON Import Format

```json
{
  "languages": [
    {
      "code": "en",
      "displayName": "English",
      "isRTL": false
    },
    {
      "code": "es",
      "displayName": "Español",
      "isRTL": false
    }
  ],
  "translations": [
    {
      "key": "ui.button.start",
      "values": [
        { "languageCode": "en", "text": "Start" },
        { "languageCode": "es", "text": "Iniciar" }
      ]
    }
  ]
}
```

## Supported Languages

The package supports any language! Here are some common ones:

| Code | Language | RTL |
|------|----------|-----|
| en | English | No |
| es | Español (Spanish) | No |
| fr | Français (French) | No |
| de | Deutsch (German) | No |
| it | Italiano (Italian) | No |
| pt | Português (Portuguese) | No |
| ru | Русский (Russian) | No |
| ja | 日本語 (Japanese) | No |
| zh | 中文 (Chinese) | No |
| ko | 한국어 (Korean) | No |
| ar | العربية (Arabic) | Yes |
| he | עברית (Hebrew) | Yes |

## Best Practices

### Naming Convention for Keys

Use a hierarchical dot notation:

```
ui.button.start
ui.button.exit
ui.menu.title
gameplay.tutorial.step1
gameplay.tutorial.step2
settings.audio.volume
settings.graphics.quality
```

### Testing Multiple Languages

1. Add test translations in the editor
2. Create a debug menu with language switching
3. Test all UI screens in each language
4. Check for text overflow issues
5. Verify RTL languages display correctly

### Performance Tips

- LocalizationManager is a singleton - only one instance exists
- LocalizedText components cache references to avoid repeated lookups
- Language changes trigger event callbacks - use sparingly
- Store LocalizationData in Resources folder for easy loading

## Troubleshooting

### "No LocalizationData assigned"

**Solution:** Assign the LocalizationData asset to the LocalizationManager in your scene.

### "LocalizationManager not found"

**Solution:** Make sure you have a LocalizationManager GameObject in your scene. Run the Setup Wizard if needed.

### Text not updating when language changes

**Solution:**
1. Make sure the LocalizedText component is enabled
2. Check that the translation key exists in the LocalizationData
3. Verify the LocalizationManager has the correct LocalizationData assigned

### Import fails

**Solution:**
1. Check CSV format (first column must be "key")
2. Ensure file encoding is UTF-8
3. Enable "Auto Backup" in settings before importing

## API Reference

### LocalizationManager

| Method | Description |
|--------|-------------|
| `SetLanguage(string code)` | Switch to a different language |
| `GetText(string key)` | Get translated text for current language |
| `GetText(string key, string lang)` | Get translated text for specific language |
| `GetCurrentLanguageData()` | Get current language metadata |
| `GetAvailableLanguages()` | Get list of all languages |
| `HasKey(string key)` | Check if translation key exists |

| Property | Description |
|----------|-------------|
| `CurrentLanguage` | Get current language code |
| `OnLanguageChanged` | Event fired when language changes |

### LocalizedText Component

| Method | Description |
|--------|-------------|
| `SetKey(string key)` | Change translation key at runtime |
| `UpdateText()` | Manually refresh text |

| Property | Description |
|----------|-------------|
| `translationKey` | The translation key to use |
| `useTextMeshPro` | Use TMP instead of legacy Text |

### LocalizationData (ScriptableObject)

| Method | Description |
|--------|-------------|
| `GetTranslation(key, lang)` | Get specific translation |
| `SetTranslation(key, lang, text)` | Set translation value |
| `HasLanguage(code)` | Check if language exists |
| `GetAllKeys()` | Get all translation keys |

## Package Structure

```
com.umardev.localization/
├── package.json
├── README.md
├── Runtime/
│   ├── UmarDev.Localization.asmdef
│   ├── LocalizationManager.cs
│   ├── Data/
│   │   └── LocalizationData.cs
│   └── Components/
│       └── LocalizedText.cs
└── Editor/
    ├── UmarDev.Localization.Editor.asmdef
    ├── LocalizationWindow.cs
    ├── LocalizationSettings.cs
    ├── Tools/
    │   └── ImportWindow.cs
    └── Setup/
        └── LocalizationSetupWizard.cs
```

## Requirements

- Unity 2021.3 or higher
- TextMeshPro package (automatically installed)

## Support

For issues, feature requests, or contributions:
- GitHub: https://github.com/UmarShabbirDev/UnityLocalization-Support
- Email: umar@umardev.com

## License

MIT License - feel free to use in personal and commercial projects!

## Changelog

### Version 1.0.0
- Initial release
- CSV/JSON import support
- Runtime language switching
- LocalizedText component
- Setup wizard
- Editor management window
- RTL language support
- Font per language support
- Auto backup system

---

**Made with ❤️ by UmarDev**
