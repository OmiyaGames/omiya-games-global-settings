# Change Log:

## 1.0.0-pre.1

- Initial release:
    - Added [`BaseSettingsData.cs`](/Runtime/BaseSettingsData.cs): an abstract `ScriptableObject` and template for storing project-wide settings.
    - Added [`BaseSettingsEditor.cs`](/Editor/BaseSettingsEditor.cs): an abstract `SettingsProvider` and template to draw the contents of a `BaseSettingsData`.
    - Added [`BaseSettingsManager.cs`](/Runtime/BaseSettingsManager.cs): an abstract `MonoBehaviour` and template for a singleton script that loads a `BaseSettingsData` instance and performs any static functions.
    - Added [`BaseSettingsBuilder.cs`](/Editor/BaseSettingsBuilder.cs): an abstract `IPreprocessBuildWithReport` and `IPostprocessBuildWithReport` script that drops an instance of `BaseSettingsData` into preloaded assets list.
