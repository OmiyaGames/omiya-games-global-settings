using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

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
		public static readonly GUIContent ACTIVE_SETTINGS_MSG = EditorGUIUtility.TrTextContent("Active Settings", "The settings that will be used by this project and included into any builds.");
		public static readonly GUIContent CREATE_BTN = EditorGUIUtility.TrTextContent("Create...");
		public const string INDENT_CLASS = ".indent";

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
		/// The path to the UXML file to display
		/// <typeparamref name="TData"/>
		/// </summary>
		public abstract string UxmlPath { get; }
		/// <summary>
		/// The path to the USS stylesheet.
		/// Used for "No Settings Available" message.
		/// </summary>
		public virtual string UssPath =>
			"Packages/com.omiyagames.global.settings/Editor/default-style.uss";
		/// <summary>
		/// The message to display if there are no settings
		/// file available.
		/// </summary>
		public virtual string NoSettingsMsg =>
			"You have no active settings for this feature. Would you like to create one?";
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
				if (value == null)
				{
					EditorBuildSettings.RemoveConfigObject(ConfigName);
				}
				else
				{
					EditorBuildSettings.AddConfigObject(ConfigName, value, true);
				}
			}
		}

		/// <summary>
		/// Creates a new instance of
		/// <typeparamref name="TData"/>.
		/// Called when the "Create..."
		/// button is clicked.
		/// </summary>
		/// <returns></returns>
		public virtual TData CreateNewSettings()
		{
			TData returnSettings = null;
			string filePath = EditorUtility.SaveFilePanel("Save Settings", "Settings", ConfigName, ".asset");
			if (string.IsNullOrEmpty(filePath) == false)
			{
				returnSettings = ScriptableObject.CreateInstance<TData>();
				AssetDatabase.CreateAsset(returnSettings, filePath);
				AssetDatabase.SaveAssetIfDirty(returnSettings);
			}
			return returnSettings;
		}

		/// <inheritdoc/>
		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			// This function is called when the user clicks on the MyCustom element in the Settings window.
			VisualElement fullTree;
			if (ActiveSettings != null)
			{
				// Import UXML
				VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);

				// Apply the UXML to the root element
				fullTree = visualTree.CloneTree();
				rootElement.Add(fullTree);

				// Bind the UXML to a serialized object
				// Note: this must be done last
				var serializedSettings = new SerializedObject(ActiveSettings);
				rootElement.Bind(serializedSettings);
			}
			else
			{
				fullTree = GetNoSettingsElements();
				rootElement.Add(fullTree);
			}

			// Check if search needs to be applied
			if (string.IsNullOrEmpty(searchContext) == false)
			{
				SettingsEditorHelpers.UpdateElementVisibility(fullTree, searchContext);
			}
		}

		/// <summary>
		/// Creates a <see cref="VisualElement"/> tree
		/// displaying message to create a settings asset.
		/// </summary>
		protected VisualElement GetNoSettingsElements()
		{
			// Setup stylesheet and return variable
			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
			VisualElement fullTree = new VisualElement();
			fullTree.AddToClassList(INDENT_CLASS);
			fullTree.styleSheets.Add(styleSheet);

			// Append object field that takes in TData
			var activeSettingsField = new ObjectField(ACTIVE_SETTINGS_MSG.text);
			activeSettingsField.tooltip = ACTIVE_SETTINGS_MSG.tooltip;
			activeSettingsField.allowSceneObjects = false;
			activeSettingsField.objectType = typeof(TData);
			activeSettingsField.value = ActiveSettings;
			activeSettingsField.RegisterCallback<ChangeEvent<Object>>(e =>
			{
				ActiveSettings = e.newValue as TData;
			});
			fullTree.Add(activeSettingsField);

			// Append the help-box
			var noSettingsHelpBox = new HelpBox(NoSettingsMsg, HelpBoxMessageType.Info);
			noSettingsHelpBox.StretchToParentWidth();
			fullTree.Add(noSettingsHelpBox);

			// Append the create button
			var createSettingsButton = new Button(() =>
			{
				TData created = CreateNewSettings();
				if (created != null)
				{
					ActiveSettings = created;
				}
			});
			createSettingsButton.text = CREATE_BTN.text;
			createSettingsButton.style.width = 100;
			fullTree.Add(createSettingsButton);

			return fullTree;
		}
	}
}
