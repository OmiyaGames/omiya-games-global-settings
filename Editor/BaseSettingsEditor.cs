using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
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

		/// <summary>
		/// Constructs a project-scoped <see cref="SettingsProvider"/>.
		/// </summary>
		/// <param name="sidebarDisplayName">
		/// Name displayed on the left sidebar in
		/// Project Settings window.
		/// </param>
		/// <seealso cref="SettingsProvider(string, SettingsScope, System.Collections.Generic.IEnumerable{string})"/>.
		protected BaseSettingsEditor(string sidebarDisplayName) : base(sidebarDisplayName, SettingsScope.Project) { }

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
			set
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
		/// <returns></returns>
		public virtual void CreateNewSettings(ClickEvent _ = null)
		{
			// Open the dialog for creating a new file
			string filePath = EditorUtility.SaveFilePanelInProject(CreateNewSettingsDialogTitle, DefaultSettingsFileName, FILE_EXTENSION, SaveSettingsMsg);

			// Check if the user didn't cancel
			if (string.IsNullOrEmpty(filePath) == false)
			{
				// Attempt to create the settings folder
				TData returnSettings = ScriptableObject.CreateInstance<TData>();
				returnSettings.name = DefaultSettingsFileName;
				AssetDatabase.CreateAsset(returnSettings, filePath);

				// Save the asset to the project
				AssetDatabase.SaveAssetIfDirty(returnSettings);
				ActiveSettings = returnSettings;
			}
		}

		/// <inheritdoc/>
		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			// Import UXML
			VisualTreeAsset uxmlTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_ROOT_EDITOR_PATH);
			TemplateContainer baseTree = uxmlTree.CloneTree();
			rootElement.Add(baseTree);

			// Update Header
			ProjectSettingsHeader header = baseTree.Q<ProjectSettingsHeader>("Header");
			header.text = HeaderText;
			header.HelpUrl = HelpUrl;

			// Populate the body with settings info
			BodyContent = baseTree.Q<VisualElement>("Body");
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
			activeSettings.RegisterCallback<ChangeEvent<Object>>(e => ActiveSettings = e.newValue as TData);

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
			if(BodyContent != null)
			{
				VisualElement newbody = (activeSetting != null) ? GetEditSettingsTree() : GetCreateSettingsTree();

				BodyContent.Clear();
				BodyContent.Add(newbody);
			}
		}
		#endregion
	}
}
