using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace OmiyaGames.Global.Settings.Editor
{
	/// <summary>
	/// Copied and pasted from <c>LocalizationBuildPlayer.cs</c> in
	/// <c>com.unity.localization</c> package.
	/// </summary>
	public abstract class BaseSettingsBuilder<TData> : IPreprocessBuildWithReport, IPostprocessBuildWithReport where TData : BaseSettingsData
	{
		TData settings;
		bool isInPreloadedAssets;

		/// <summary>
		/// The name the settings asset will be in project via
		/// <seealso cref="EditorBuildSettings.AddConfigObject(string, Object, bool)"/>
		/// </summary>
		/// <remarks>
		/// Should follow the format:
		/// <c>company.package.name</c>
		/// </remarks>
		public abstract string ConfigName { get; }
		/// <inheritdoc/>
		public int callbackOrder => 1;

		/// <inheritdoc/>
		public void OnPreprocessBuild(BuildReport report)
		{
			// Setup member variables
			isInPreloadedAssets = false;
			EditorBuildSettings.TryGetConfigObject(ConfigName, out TData settings);
			if (settings == null)
			{
				return;
			}

			// Add the localization settings to the preloaded assets.
			Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
			bool wasDirty = IsPlayerSettingsDirty();

			if (preloadedAssets.Contains(settings) == false)
			{
				ArrayUtility.Add(ref preloadedAssets, settings);
				PlayerSettings.SetPreloadedAssets(preloadedAssets);

				// If we have to add the settings then we should also remove them.
				isInPreloadedAssets = true;

				// Clear the dirty flag so we dont flush the modified file (case 1254502)
				if (wasDirty == false)
				{
					ClearPlayerSettingsDirtyFlag();
				}
			}
		}

		/// <inheritdoc/>
		public void OnPostprocessBuild(BuildReport report)
		{
			if ((settings == null) || (isInPreloadedAssets == false))
			{
				return;
			}

			bool wasDirty = IsPlayerSettingsDirty();

			Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
			ArrayUtility.Remove(ref preloadedAssets, settings);
			PlayerSettings.SetPreloadedAssets(preloadedAssets);

			// Reset member variables
			settings = null;

			// Clear the dirty flag so we dont flush the modified file (case 1254502)
			if (wasDirty == false)
			{
				ClearPlayerSettingsDirtyFlag();
			}
		}

		static bool IsPlayerSettingsDirty()
		{
			bool returnFlag = false;
			PlayerSettings[] settings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if ((settings != null) && (settings.Length > 0))
			{
				returnFlag = EditorUtility.IsDirty(settings[0]);
			}
			return returnFlag;
		}

		static void ClearPlayerSettingsDirtyFlag()
		{
			PlayerSettings[] settings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if ((settings != null) && (settings.Length > 0))
			{
				EditorUtility.ClearDirty(settings[0]);
			}
		}
	}
}
