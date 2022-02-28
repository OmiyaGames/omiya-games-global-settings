# Change Log:

## 1.1.0-pre.2

 - **New Enhancement:** Updating documentation on [`BaseSettingsManager.cs`](/Runtime/BaseSettingsManager.cs) encouraging a new standard: keep any extending classes private.  Updated certain method's access level, such as `GetInstance()` and `GetData()`, to public so this standard can more easily be enforced.

## 1.1.0-pre.1

 - **New Enhancement:** adding virtual method, `CustomizeEditSettingsTree(VisualElement, SerializedObject)`, in [`BaseSettingsEditor.cs`](/Editor/BaseSettingsEditor.cs). this method provides more flexibility for any classes extending this abstract class to customize the editor. This replaces `GetEditSettingsTree()`.

## 1.0.1-pre.2

 - Updating package version keywords, dependency versions, etc.

## 1.0.1-pre.1

 - **Bug Fixes:** covering edge cases in [`BaseSettingsEditor.cs`](/Editor/BaseSettingsEditor.cs) when...
     - Addressables settings is not setup, in which case a pop-up indicating so is presented;
     - If the user drag-and-drops an existing setting file, in which case the script will be added to both Editor config and Addressables.
 - **New Enhancement:** Adding new helper function, `LoadSettingsAsync<T>(string)` in [`Data.cs`](/Data/Data.cs).

## 1.0.0-pre.1

- Initial release:
    - Added [`BaseSettingsData.cs`](/Runtime/BaseSettingsData.cs): an abstract `ScriptableObject` and template for storing project-wide settings.
    - Added [`BaseSettingsEditor.cs`](/Editor/BaseSettingsEditor.cs): an abstract `SettingsProvider` and template to draw the contents of a `BaseSettingsData`.
    - Added [`BaseSettingsManager.cs`](/Runtime/BaseSettingsManager.cs): an abstract `MonoBehaviour` and template for a singleton script that loads a `BaseSettingsData` instance and performs any static functions.
    - Added [`BaseSettingsBuilder.cs`](/Editor/BaseSettingsBuilder.cs): an abstract `IPreprocessBuildWithReport` and `IPostprocessBuildWithReport` script that drops an instance of `BaseSettingsData` into preloaded assets list.
