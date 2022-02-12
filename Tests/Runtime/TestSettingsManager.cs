using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Global.Settings.Tests
{
	public class TestSettingsManager : BaseSettingsManager<TestSettingsManager, TestSettingsData>
	{
		public const string ADDRESSABLE_NAME = "TestSettings";
		protected override string AddressableName => ADDRESSABLE_NAME;
	}
}
