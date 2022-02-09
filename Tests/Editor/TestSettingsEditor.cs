using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OmiyaGames.Global.Settings.Tests;

namespace OmiyaGames.Global.Settings.Editor.Tests
{
	public class TestSettingsEditor : BaseSettingsEditor<TestSettingsData>
	{
		public const string SIDEBAR_DISPLAY_PATH = "Project/Omiya Games/Test Editor";

		/// <inheritdoc/>
		public TestSettingsEditor(string sidebarDisplayName) : base(sidebarDisplayName) { }

		/// <inheritdoc/>
		public override string ConfigName => "com.omiyagames.global.settings";

		/// <inheritdoc/>
		public override string UxmlPath => "Packages/com.omiyagames.global.settings/Tests/Editor/TestSettingsEditor.uxml";

		/// <inheritdoc/>
		public override string HeaderText => "This is a Test!";

		/// <inheritdoc/>
		public override string HelpUrl => "https://omiyagames.com";

		/// <inheritdoc/>
		public override string DefaultSettingsFileName => "TestSettings";

		/// <summary>
		/// Registers this <see cref="SettingsProvider"/>.
		/// </summary>
		/// <returns></returns>
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			// Create the settings provider
			var returnProvider = new TestSettingsEditor(SIDEBAR_DISPLAY_PATH);
			return returnProvider;
		}
	}
}
