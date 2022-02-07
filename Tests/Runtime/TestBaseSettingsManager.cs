using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;
using OmiyaGames.Global;

namespace OmiyaGames.Global.Settings.Tests
{
	public class TestBaseSettingsManager
	{
		/// <summary>
		/// Unit test 
		/// </summary>
		/// <seealso cref="RandomList{T}.RandomList(IEqualityComparer{T})"/>
		[UnityTest]
		public IEnumerator TestLoadingDataFailed()
		{
			// Wait until is done loading
			yield return TestSettingsManager.WaitUntilReady();

			// Check if the default data is being used by TestSettingsManager.
			Assert.AreEqual(Data.Status.UsingDefaultData, TestSettingsManager.DataStatus);
			ComponentSingleton<TestSettingsManager>.Release();
		}
	}
}
