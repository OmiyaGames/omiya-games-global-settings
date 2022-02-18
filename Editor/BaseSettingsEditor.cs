using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Global.Settings.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="BaseSettingsEditor.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2022 Omiya Games
	/// 
	/// Permission is hereby granted, free of charge, to any person obtaining a copy
	/// of this software and associated documentation files (the "Software"), to deal
	/// in the Software without restriction, including without limitation the rights
	/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	/// copies of the Software, and to permit persons to whom the Software is
	/// furnished to do so, subject to the following conditions:
	/// 
	/// The above copyright notice and this permission notice shall be included in
	/// all copies or substantial portions of the Software.
	/// 
	/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	/// THE SOFTWARE.
	/// </copyright>
	/// <list type="table">
	/// <listheader>
	/// <term>Revision</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>
	/// <strong>Date:</strong> 2/7/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Initial version.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Template editor script for any <seealso cref="BaseSettingsData"/>.
	/// </summary>
	/// <typeparam name="TData">
	/// Data to render to the Project Settings window.
	/// </typeparam>
	public abstract partial class BaseSettingsEditor<TData> : SettingsProvider where TData : BaseSettingsData
	{
		public const string FILE_EXTENSION = "asset";
		const string UXML_DIR = "Packages/com.omiyagames.global.settings/Editor/";
		const string UXML_ROOT_EDITOR_PATH = UXML_DIR + "SettingsEditorRoot.uxml";
		const string UXML_CREATE_SETTINGS_EDITOR_PATH = UXML_DIR + "CreateSettingsBody.uxml";
		const string OK = "OK";

		/// <summary>
		/// Constructs a project-scoped <see cref="SettingsProvider"/>.
		/// </summary>
		/// <param name="sidebarDisplayName">
		/// Name displayed on the left sidebar in
		/// Project Settings window.
		/// </param>
		/// <seealso cref="SettingsProvider(string, SettingsScope, System.Collections.Generic.IEnumerable{string})"/>.
		protected BaseSettingsEditor(string sidebarDisplayName, System.Collections.Generic.IEnumerable<string> keywords = null) : base(sidebarDisplayName, SettingsScope.Project, keywords) { }

		/// <summary>
		/// The name of the group the addressable will be placed under.
		/// </summary>
		public abstract string AddressableGroupName { get; }
		/// <summary>
		/// The name of the addressable.  This can be loaded from
		/// <seealso cref"Addressables.LoadAssetAsync{TData}(string)">
		/// </summary>
		public abstract string AddressableName { get; }
		/// <summary>
		/// The name the settings asset will be in project via
		/// <seealso cref="EditorBuildSettings.AddConfigObject(string, Object, bool)"/>
		/// </summary>
		/// <remarks>
		/// Should follow the format:
		/// <c>company.package.name</c>
		/// </remarks>
		public abstract string ConfigName { get; }
		/// <summary>
		/// The text to replace the header.
		/// </summary>
		public abstract string HeaderText { get; }
		/// <summary>
		/// The url to help assist users.
		/// </summary>
		public abstract string HelpUrl { get; }
		/// <summary>
		/// The path to the UXML file, used to edit
		/// <typeparamref name="TData"/>
		/// </summary>
		public abstract string UxmlPath { get; }
		/// <summary>
		/// The default settings file name, without the file extension.
		/// </summary>
		public abstract string DefaultSettingsFileName { get; }
		/// <summary>
		/// The message to display if there are no settings
		/// file available.
		/// </summary>
		public virtual string NoSettingsMsg =>
			"You have no active settings for this feature. Would you like to create one?";
		/// <summary>
		/// The title of the dialog that appears in the pop-up after
		/// clicking "Create...'
		/// </summary>
		public virtual string CreateNewSettingsDialogTitle => "Save Settings";
		/// <summary>
		/// The message to display in the save settings dialog.
		/// </summary>
		public virtual string SaveSettingsMsg =>
			"Please enter a filename to save this settings to.";
		/// <summary>
		/// Gets or sets <typeparamref name="TData"/> stored in.
		/// <seealso cref="EditorBuildSettings"/>.
		/// </summary>
		public TData ActiveSettings
		{
			get
			{
				EditorBuildSettings.TryGetConfigObject(ConfigName, out TData settings);
				return settings;
			}
			private set
			{
				// Update the editor build settings
				if (value == null)
				{
					EditorBuildSettings.RemoveConfigObject(ConfigName);
				}
				else
				{
					EditorBuildSettings.AddConfigObject(ConfigName, value, true);
				}

				// Update UI
				UpdateBodyContent(value);
				Repaint();
			}
		}
		/// <summary>
		/// The settings content tree.
		/// </summary>
		protected VisualElement BodyContent
		{
			get;
			private set;
		} = null;

		/// <summary>
		/// Creates a new instance of
		/// <typeparamref name="TData"/>.
		/// Called when the "Create..."
		/// button is clicked.
		/// </summary>
		public virtual void CreateNewSettings(ClickEvent _ = null)
		{
			// Attempt to retrieve the addressable settings, first
			AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
			if (addressableSettings == null)
			{
				// If none is found, complain
				WarnAddressableNotSetup();
				return;
			}

			// Open the dialog for creating a new file
			string filePath = EditorUtility.SaveFilePanelInProject(CreateNewSettingsDialogTitle, DefaultSettingsFileName, FILE_EXTENSION, SaveSettingsMsg);

			// Check if the user didn't cancel
			if (string.IsNullOrEmpty(filePath) == false)
			{
				// Create the new settings
				CreateNewSettingsAt(filePath, addressableSettings);
			}
		}

		/// <inheritdoc/>
		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			// Import UXML
			VisualTreeAsset uxmlTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_ROOT_EDITOR_PATH);
			uxmlTree.CloneTree(rootElement);

			// Update Header
			ProjectSettingsHeader header = rootElement.Q<ProjectSettingsHeader>("Header");
			header.text = HeaderText;
			header.HelpUrl = HelpUrl;

			// Populate the body with settings info
			BodyContent = rootElement.Q<VisualElement>("Body");
			UpdateBodyContent(ActiveSettings);
		}

		/// <summary>
		/// Creates a <see cref="VisualElement"/> tree
		/// to edit the settings asset.
		/// </summary>
		protected virtual VisualElement GetEditSettingsTree()
		{
			// Import UXML
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
			VisualElement returnTree = visualTree.CloneTree();

			// Bind the UXML to a serialized object
			// Note: this must be done last
			var serializedSettings = new SerializedObject(ActiveSettings);
			returnTree.Bind(serializedSettings);
			return returnTree;
		}

		/// <summary>
		/// Creates a new instance of
		/// <typeparamref name="TData"/>.
		/// Called when the "Create..."
		/// button is clicked.
		/// </summary>
		/// <param name="filePath">
		/// The path to create the new file in.
		/// </param>
		/// <param name="addressableSettings">
		/// The addressable settings.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// If either arguments are null or if
		/// <paramref name="filePath"/> is empty string.
		/// </exception>
		protected virtual TData CreateNewSettingsAt(string filePath, AddressableAssetSettings addressableSettings)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new System.ArgumentNullException("addressableSettings");
			}
			else if (addressableSettings == null)
			{
				throw new System.ArgumentNullException("addressableSettings");
			}

			// Setup some helper methods
			const string PROGRESS_TITLE = "Creating Settings";
			float GetProgress(int step) => Mathf.Clamp01(((float)step) / 6f);
			TData returnSettings = null;

			try
			{
				// Start progress bar
				EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Creating New File...", GetProgress(0));

				// Attempt to create the settings folder
				returnSettings = ScriptableObject.CreateInstance<TData>();
				returnSettings.name = DefaultSettingsFileName;
				AssetDatabase.CreateAsset(returnSettings, filePath);

				// Save the asset to the project
				EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Saving Asset...", GetProgress(1));
				AssetDatabase.SaveAssetIfDirty(returnSettings);
				EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Update Editor Settings...", GetProgress(2));

				// Attempt to get an addressable group
				EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Get Addressable Group...", GetProgress(3));
				AddressableAssetGroup addressableGroup = addressableSettings.FindGroup(AddressableGroupName);
				if (addressableGroup == null)
				{
					// Group not found, create a new read-only group
					EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Create New Addressable Group...", GetProgress(4));
					addressableGroup = addressableSettings.CreateGroup(AddressableGroupName, false, true, false, addressableSettings.DefaultGroup.Schemas);
					addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, addressableGroup, true);
				}

				// Add the asset to the group
				EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Create New Addressable File...", GetProgress(5));
				string sourceGuid = AssetDatabase.AssetPathToGUID(filePath);
				AddressableAssetEntry entry = addressableSettings.CreateOrMoveEntry(sourceGuid, addressableGroup, true);
				entry.address = AddressableName;
				EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Update Addressables UI...", GetProgress(6));
				addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

				// Close progress bar
				EditorUtility.ClearProgressBar();
			}
			catch (System.Exception ex)
			{
				// Close progress bar
				EditorUtility.ClearProgressBar();

				if (returnSettings)
				{
					// Remove returnSettings data
					Object.DestroyImmediate(returnSettings);
				}

				// Complain about the exception
				Debug.LogError(ex);
				EditorUtility.DisplayDialog("Unable to Create Settings",
					$"There was an error attempting to create a settings file at \"{filePath}\".  Please confirm you have permissions to write to this path.", OK);

				// Halt
				return null;
			}

			// Update settings
			ActiveSettings = returnSettings;
			return returnSettings;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="existingSettings"></param>
		protected virtual void OnUserChangedActiveSettings(TData existingSettings)
		{
			// Check if user provided settings isn't null
			if (existingSettings != null)
			{
				// Attempt to retrieve the addressable settings, first
				AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
				if (addressableSettings == null)
				{
					// If none is found, complain
					WarnAddressableNotSetup();
					return;
				}

				// Setup some helper methods
				const string PROGRESS_TITLE = "Creating Settings";
				float GetProgress(int step) => Mathf.Clamp01(((float)step) / 3f);

				try
				{
					// Attempt to get an addressable group
					EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Get Addressable Group...", GetProgress(0));
					AddressableAssetGroup addressableGroup = addressableSettings.FindGroup(AddressableGroupName);
					if (addressableGroup == null)
					{
						// Group not found, create a new read-only group
						EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Create New Addressable Group...", GetProgress(1));
						addressableGroup = addressableSettings.CreateGroup(AddressableGroupName, false, true, false, addressableSettings.DefaultGroup.Schemas);
						addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, addressableGroup, true);
					}

					// Add the asset to the group
					EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Add File to Addressable...", GetProgress(2));
					AssetDatabase.TryGetGUIDAndLocalFileIdentifier(existingSettings, out string sourceGuid, out long localId);
					AddressableAssetEntry entry = addressableSettings.CreateOrMoveEntry(sourceGuid, addressableGroup, true);
					entry.address = AddressableName;
					EditorUtility.DisplayProgressBar(PROGRESS_TITLE, "Update Addressables UI...", GetProgress(3));
					addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

					// Close progress bar
					EditorUtility.ClearProgressBar();
				}
				catch (System.Exception ex)
				{
					// Close progress bar
					EditorUtility.ClearProgressBar();

					// Complain about the exception
					Debug.LogError(ex);
					EditorUtility.DisplayDialog("Unable to Add Settings to Addressables",
						$"There was an error attempting to add the settings into Addressables.", OK);
					return;
				}
			}

			// Update settings
			ActiveSettings = existingSettings;
		}

		/// <summary>
		/// TODO
		/// </summary>
		protected static void WarnAddressableNotSetup()
		{
			EditorUtility.DisplayDialog("Addressables Is Not Setup",
				"This settings requires Addressables to be setup. Generate Addressable settings first (e.g. by creating localization settings) before creating an Omiya Games settings.", OK);
		}

		#region Helper Methods
		/// <summary>
		/// Creates a <see cref="VisualElement"/> tree
		/// displaying message to create a settings asset.
		/// </summary>
		VisualElement GetCreateSettingsTree()
		{
			// Grab the body
			VisualTreeAsset uxmlTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_CREATE_SETTINGS_EDITOR_PATH);
			VisualElement returnTree = uxmlTree.CloneTree();

			// Update Object Field
			ObjectField activeSettings = returnTree.Q<ObjectField>("ActiveSettings");
			activeSettings.objectType = typeof(TData);
			activeSettings.value = ActiveSettings;
			activeSettings.RegisterCallback<ChangeEvent<Object>>(e => OnUserChangedActiveSettings(e.newValue as TData));

			// Update help info
			VisualElement helpInfo = returnTree.Q<VisualElement>("HelpInfo");
			var noSettingsHelpBox = new HelpBox(NoSettingsMsg, HelpBoxMessageType.Info);
			helpInfo.Add(noSettingsHelpBox);

			// Register CreateNewSettings into Create button
			Button createButton = returnTree.Q<Button>("CreateButton");
			createButton.RegisterCallback<ClickEvent>(CreateNewSettings);
			return returnTree;
		}

		void UpdateBodyContent(TData activeSetting)
		{
			// Redraw this UI
			if (BodyContent != null)
			{
				VisualElement newbody = (activeSetting != null) ? GetEditSettingsTree() : GetCreateSettingsTree();

				BodyContent.Clear();
				BodyContent.Add(newbody);
			}
		}
		#endregion
	}
}
