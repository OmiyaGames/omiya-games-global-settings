using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Global.Settings.Tests
{
	public class TestSettingsManager : BaseSettingsManager<TestSettingsManager, TestSettingsData>
	{
		protected override string DataPath => "Assets/Settings/Test";
	}
}
