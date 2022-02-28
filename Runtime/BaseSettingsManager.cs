using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 1.1.0-pre.2<br/>
	/// <strong>Date:</strong> 2/6/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Making the script returned by <see cref="GetInstance"/> deactivated
	/// by default. User will have to override <see cref="OnSetup"/> to
	/// activate the script on start.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// <para>
	/// Abstract class that manages game interaction with
	/// <typeparamref name="TData"/>.
	/// </para><para>
	/// Most classes extending this should be a private class that a static
	/// class access:
	/// <code>
	/// public static class TestManager
	/// {
	///     // Add static methods and properties here
	///     public static int CurrentVersion => TestSettingsManager.GetData().CurrentVersion;
	/// 
	///     // Creating a private implementation of BaseSettingsManager so
	///     // it doesn't show up as a script that can be attached to a GameObject
	///     private class TestSettingsManager : BaseSettingsManager&lt;TestSettingsManager, TestData&gt;
	///     {
	///         // Return the expected addressable name here
	///         protected override string AddressableName => "Test";
	/// 
	///         // Optionally, override the OnSetup method to perform any setup
	///         protected override IEnumerator OnSetup()
	///         {
	///             // The base method loads the data
	///             yield return Manager.StartCoroutine(base.OnSetup());
	/// 
	///             // Perform some setup here
	///             Debug.Log($"Data version is {Data.CurrentVersion}");
	///         }
	///     }
	/// 
	///     // BaseSettingsData can be public, though;
	///     // it's just a ScriptableObject.
	///     public class TestData : BaseSettingsData { }
	/// }
	/// </code>
	/// </para>
	/// </summary>
	/// <typeparam name="TManager">
	/// The concrete type extending this class.
	/// </typeparam>
	/// <typeparam name="TData">
	/// The type of stored project settings to read from.
	/// </typeparam>
	public abstract class BaseSettingsManager<TManager, TData> : MonoBehaviour where TManager : BaseSettingsManager<TManager, TData> where TData : BaseSettingsData
	{
		AsyncOperationHandle<TData> loadDataHandle;

		/// <summary>
		/// Grabs the static instance of this manager.
		/// </summary>
		/// <seealso cref="GetData()"/>
		public static TManager GetInstance()
		{
			TManager returnInstance = ComponentSingleton<TManager>.Get(out bool isFirstTimeCreated);
			if (isFirstTimeCreated)
			{
				Manager.StartCoroutine(returnInstance.OnSetup());
			}
			return returnInstance;
		}

		/// <summary>
		/// Grabs the project settings data.
		/// </summary>
		/// <seealso cref="GetInstance()"/>
		/// <seealso cref="IsReady"/>
		public static TData GetData() => GetInstance().Data;

		/// <summary>
		/// Current status of whether
		/// data for this manager has been loaded or not
		/// </summary>
		public static Data.Status GetDataStatus()
		{
			switch (GetInstance().loadDataHandle.Status)
			{
				case AsyncOperationStatus.Succeeded:
					return Settings.Data.Status.RetrievedProjectData;
				case AsyncOperationStatus.Failed:
					return Settings.Data.Status.UsingDefaultData;
				default:
					return Settings.Data.Status.NowLoading;
			}
		}

		/// <summary>
		/// Created a coroutine that waits until this manager
		/// finished loading its data.
		/// </summary>
		/// <seealso cref="IsReady"/>
		public static IEnumerator WaitUntilReady()
		{
			TManager check = GetInstance();
			if (check.Data == null)
			{
				yield return new WaitUntil(() => check.Data != null);
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
		/// Grabs the data.
		/// </summary>
		protected TData Data
		{
			get;
			private set;
		}

		/// <summary>
		/// Event called on creation of this manager
		/// (before <c>Awake()</c> and <c>Start()</c> are called.)
		/// </summary>
		protected virtual IEnumerator OnSetup()
		{
			// Attempt to grab a reference to the data
			loadDataHandle = Settings.Data.LoadSettingsAsync<TData>(AddressableName);

			// Wait until it's done loading
			yield return loadDataHandle;

			// Check the status
			if (loadDataHandle.Status == AsyncOperationStatus.Succeeded)
			{
				// Set data to the results
				Data = loadDataHandle.Result;
			}
			else
			{
				// Set data to a default instance
				Data = ScriptableObject.CreateInstance<TData>();
			}

			// Wait a frame
			yield return null;
		}

		/// <summary>
		/// Event called on disposing this manager.
		/// </summary>
		protected virtual void OnDestroy()
		{
			// Destroy default data
			if (GetDataStatus() == Settings.Data.Status.UsingDefaultData)
			{
				Destroy(Data);
			}

			// Release the handle
			Addressables.Release(loadDataHandle);
		}
	}
}
