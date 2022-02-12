using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DataHelper = OmiyaGames.Global.Settings.Data;

namespace OmiyaGames.Global.Settings
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="BaseSettingsManager.cs" company="Omiya Games">
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
	/// <strong>Version:</strong> 1.0.0-pre.1<br/>
	/// <strong>Date:</strong> 2/6/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Abstract class that manages game interaction with
	/// <typeparamref name="TData"/>.
	/// </summary>
	/// <typeparam name="TManager">
	/// The concrete type extending this class.
	/// </typeparam>
	/// <typeparam name="TData">
	/// The type of stored project settings to read from.
	/// </typeparam>
	/// <remarks>
	/// Most classes extending this one should be composed of static methods
	/// and properties:
	/// <code>
	/// public class TimeManager : BaseSettingsManager&lt;TimeManager, TimeData&gt;
	/// {
	///     // Add static methods here
	/// }
	/// </code>
	/// </remarks>
	public abstract class BaseSettingsManager<TManager, TData> : MonoBehaviour where TManager : BaseSettingsManager<TManager, TData> where TData : BaseSettingsData
	{
		TData data = null;
		AsyncOperationHandle<TData> loadDataHandle;

		/// <summary>
		/// Grabs the static instance of this manager.
		/// </summary>
		/// <seealso cref="Data"/>
		protected static TManager Instance
		{
			get
			{
				TManager returnInstance = ComponentSingleton<TManager>.Get(true, out bool isFirstTimeCreated);
				if (isFirstTimeCreated)
				{
					returnInstance.StartCoroutine(returnInstance.OnSetup());
				}
				return returnInstance;
			}
		}

		/// <summary>
		/// Grabs the project settings data.
		/// </summary>
		/// <seealso cref="Instance"/>
		/// <seealso cref="IsReady"/>
		protected static TData Data => Instance.data;

		/// <summary>
		/// Current status of whether
		/// data for this manager has been loaded or not
		/// </summary>
		public static DataHelper.Status DataStatus
		{
			get
			{
				switch (Instance.loadDataHandle.Status)
				{
					case AsyncOperationStatus.Succeeded:
						return DataHelper.Status.RetrievedProjectData;
					case AsyncOperationStatus.Failed:
						return DataHelper.Status.UsingDefaultData;
					default:
						return DataHelper.Status.NowLoading;
				}
			}
		}

		/// <summary>
		/// Created a coroutine that waits until this manager
		/// finished loading its data.
		/// </summary>
		/// <seealso cref="IsReady"/>
		public static IEnumerator WaitUntilReady()
		{
			TManager check = Instance;
			while (check.data == null)
			{
				yield return null;
			}
		}

		/// <summary>
		/// Gets the name of the manager setting's addessable.
		/// </summary>
		protected abstract string AddressableName
		{
			get;
		}

		/// <summary>
		/// Event called on creation of this manager
		/// (before <c>Awake()</c> and <c>Start()</c> are called.)
		/// </summary>
		protected virtual IEnumerator OnSetup()
		{
			// Attempt to grab a reference to the data
			loadDataHandle = Addressables.LoadAssetAsync<TData>(AddressableName);

			// Wait until it's done loading
			yield return loadDataHandle;

			// Check the status
			if (loadDataHandle.Status == AsyncOperationStatus.Succeeded)
			{
				// Set data to the results
				data = loadDataHandle.Result;
			}
			else
			{
				// Set data to a default instance
				data = ScriptableObject.CreateInstance<TData>();
			}
		}

		/// <summary>
		/// Event called on disposing this manager.
		/// </summary>
		protected virtual void OnDestroy()
		{
			// Destroy default data
			if (DataStatus == DataHelper.Status.UsingDefaultData)
			{
				Destroy(data);
			}

			// Release the handle
			Addressables.Release(loadDataHandle);
		}
	}
}
