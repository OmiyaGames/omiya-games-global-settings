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
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 1.2.0-pre.1<br/>
	/// <strong>Date:</strong> 3/3/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// <para>
	/// Adding breaking changes:
	/// </para><para>
	/// Replacing coroutine <c>WaitUntilRead()</c> with <see cref="Setup(bool)"/>.
	/// </para><para>
	/// Adding virtual method <see cref="GetStatus()"/>. This needs to be
	/// overridden if <see cref="OnSetup()"/> is overridden as well, as
	/// <see cref="Setup(bool)"/> uses <see cref="GetStatus()"/> to determine
	/// when <see cref="OnSetup()"/> method is done.  See example code below
	/// for more details.
	/// </para><para>
	/// Added helper methods <see cref="GetInstanceOrThrow()"/> and <see cref="GetDataOrThrow()"/>.
	/// </para>
	/// </description>
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
	/// <example>
	/// Most classes extending this should be a private class that a static
	/// class access:
	/// <code>
	/// using System.Collections;
	/// 
	/// public static class TestManager
	/// {
	///     // Add static methods and properties here
	///     public static IEnumerator Setup(bool force)
	///     {
	///         // This coroutine ends when GetStatus() returns
	///         // a value other than Data.Status.Loading
	///         yield return Manager.StartCoroutine(TestSettingsManager.Setup(force));
	///     }
	/// 
	///     public static int CurrentVersion => TestSettingsManager.GetDataOrThrow().CurrentVersion;
	/// 
	///     // Creating a private implementation of BaseSettingsManager so
	///     // it doesn't show up as a script that can be attached to a GameObject
	///     class TestSettingsManager : BaseSettingsManager&lt;TestSettingsManager, TestData&gt;
	///     {
	///         // Return the expected addressable name here
	///         protected override string AddressableName => "Test";
	/// 
	///         // Optionally, override the OnSetup method to perform any setup
	///         Data.Status status = Data.Status.Loading;
	/// 
	///         protected override IEnumerator OnSetup()
	///         {
	///             // Indicate loading
	///             status = Data.Status.Loading;
	/// 
	///             // The base method loads the data
	///             yield return Manager.StartCoroutine(base.OnSetup());
	/// 
	///             // Perform some setup here
	///             Debug.Log($"Data version is {Data.CurrentVersion}");
	/// 
	///             // Indicate loading finished
	///             status = base.GetStatus();
	///         }
	/// 
	///         // If OnSetup method is overridden, don't forget to override GetStatus as well.
	///         // If this method returns a value other than Data.Status.Loading, Setup(bool)
	///         // assumes OnSetup is finished (though not necessarily setup successfully.)
	///         public override Data.Status GetStatus() => status;
	///     }
	/// 
	///     // BaseSettingsData can be public, though;
	///     // it's just a ScriptableObject.
	///     public class TestData : BaseSettingsData { }
	/// }
	/// </code>
	/// </example>
	public abstract class BaseSettingsManager<TManager, TData> : MonoBehaviour where TManager : BaseSettingsManager<TManager, TData> where TData : BaseSettingsData
	{
		AsyncOperationHandle<TData> loadDataHandle;
		WaitWhile waitCache;

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
		public static Data.Status GetDataStatus() => GetInstance().GetStatus();

		/// <summary>
		/// Same thing as <seealso cref="GetInstance()"/>,
		/// but throws an exception if not setup correctly.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		public static TManager GetInstanceOrThrow()
		{
			TManager manager = GetInstance();
			switch (manager.GetStatus())
			{
				case Settings.Data.Status.Success:
				case Settings.Data.Status.UsingDefaultData:
					return manager;
				default:
					throw new System.Exception($"{typeof(TManager)} is not setup yet.");
			}
		}

		/// <summary>
		/// Same thing as <seealso cref="GetData()"/>,
		/// but throws an exception if not setup correctly.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		public static TData GetDataOrThrow() => GetInstanceOrThrow().Data;

		/// <summary>
		/// A coroutine to setup this manager.
		/// </summary>
		/// <param name="forceSetup">
		/// If true, forces the manager to perform setup again, assuming
		/// it already is in the middle of that.
		/// </param>
		/// <returns></returns>
		public static IEnumerator Setup(bool forceSetup = false)
		{
			// Check if currently loading
			TManager manager = GetInstance();
			if (IsLoading())
			{
				// Create a Wait instance, and cache it
				if (manager.waitCache == null)
				{
					manager.waitCache = new WaitWhile(IsLoading);
				}

				// Return the cache
				yield return manager.waitCache;
			}
			else if (forceSetup)
			{
				// Force setup
				yield return Manager.StartCoroutine(manager.OnSetup());
			}

			bool IsLoading() => (manager.GetStatus() == Settings.Data.Status.Loading);
		}

		/// <summary>
		/// Gets the name of the manager setting's addessable.
		/// </summary>
		protected abstract string AddressableName
		{
			get;
		}

		/// <summary>
		/// Retrieves the current status of this manager.
		/// </summary>
		/// <returns></returns>
		public virtual Data.Status GetStatus()
		{
			switch (loadDataHandle.Status)
			{
				case AsyncOperationStatus.Succeeded:
					return Settings.Data.Status.Success;
				case AsyncOperationStatus.Failed:
					return Settings.Data.Status.UsingDefaultData;
				default:
					return Settings.Data.Status.Loading;
			}
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
		/// <remarks>
		/// If this method is overridden, don't forget to override
		/// <seealso cref="GetStatus()"/> as well.
		/// </remarks>
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
