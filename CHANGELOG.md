# Changelog

All notable changes to the UmarDev Localization Pro package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-05-14

### Added
- Initial release of UmarDev Localization Pro
- LocalizationData ScriptableObject for storing translations
- LocalizationManager singleton for runtime language management
- LocalizedText component for automatic UI text updates
- Main EditorWindow for managing translations
- CSV import tool with format validation
- JSON import tool for structured data
- Setup wizard for first-time configuration
- Settings manager with EditorPrefs persistence
- Support for multiple languages with custom fonts
- RTL (Right-to-Left) language support
- Language preference saving with PlayerPrefs
- Search and filter functionality in editor
- Auto-backup system before imports
- Comprehensive documentation and README
- API reference and usage examples
- Common language presets (English, Spanish, French, German, Japanese, Arabic, etc.)

### Features
- Runtime language switching without scene reload
- Event system for language change notifications
- Font swapping per language (Unity UI and TextMeshPro)
- Hierarchical key naming convention support
- Translation key validation
- Missing translation detection with warnings
- Scene manager auto-creation
- DontDestroyOnLoad support for persistent manager
- Editor-time preview of translations
- Batch translation import from spreadsheets

### Technical
- Assembly definitions for clean namespace separation
- Unity Package Manager (UPM) compatible
- Minimum Unity version: 2021.3
- TextMeshPro integration
- EditorWindow-based UI with scrolling support
- Undo/Redo support for editor operations
- Asset database integration for proper Unity workflow

## [Unreleased]

### Planned Features
- Google Sheets integration for live translation updates
- Pluralization support (singular/plural forms)
- String formatting with parameters (e.g., "Hello {0}!")
- Translation memory and suggestions
- Missing translation report generator
- Export to CSV/JSON
- Localization of sprites and audio clips
- Automatic text size adjustment for different languages
- Translation progress tracking per language
- Collaboration features for team workflows

---

For more information, visit: https://github.com/UmarShabbirDev/UnityLocalization-Support
