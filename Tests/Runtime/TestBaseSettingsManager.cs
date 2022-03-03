using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;
using OmiyaGames.Global;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OmiyaGames.Global.Settings.Tests
{
	public class TestBaseSettingsManager
	{
		/// <summary>
		/// Unit test 
		/// </summary>
		/// <seealso cref="RandomList{T}.RandomList(IEqualityComparer{T})"/>
		[UnityTest]
		public IEnumerator TestDataStatus()
		{
			// Grab the actual data first
			AsyncOperationHandle<TestSettingsData> handle = Addressables.LoadAssetAsync<TestSettingsData>(TestSettingsManager.ADDRESSABLE_NAME);

			// Wait until it's done loading
			yield return handle;

			// Retrieve the status of this data
			Data.Status expectedStatus = Data.Status.UsingDefaultData;
			if(handle.Status == AsyncOperationStatus.Succeeded)
			{
				expectedStatus = Data.Status.Success;
			}

			// Release the handle
			Addressables.Release(handle);

			// Wait until is done loading
			yield return TestSettingsManager.Setup();

			// Check if the default data is being used by TestSettingsManager.
			Assert.AreEqual(expectedStatus, TestSettingsManager.GetDataStatus());
			ComponentSingleton<TestSettingsManager>.Release();
		}
	}
}
