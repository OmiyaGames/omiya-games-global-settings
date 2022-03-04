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
	/// <strong>Version:</strong> 1.0.1-pre.1<br/>
	/// <strong>Date:</strong> 2/19/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Adding helper method, <seealso cref="LoadSettingsAsync"/>.
	/// </description>
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 1.2.0-pre.1<br/>
	/// <strong>Date:</strong> 3/3/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Adding breaking changes: <c>Status.NowLoading</c> has been replaced
	/// with <see cref="Status.Loading"/>; <c>Status.RetrievedProjectData</c>,
	/// with <see cref="Status.Success"/>; and added <see cref="Status.Fail"/>.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Helper set of info.
	/// </summary>
	public static class Data
	{
		/// <summary>
		/// Attempts to load an addressable asynchronously.
		/// </summary>
		/// <param name="address">
		/// The name of the addressable.
		/// </param>
		/// <returns>
		/// Coroutine retrieve the addressable.
		/// </returns>
		public static AsyncOperationHandle<T> LoadSettingsAsync<T>(string address) where T : BaseSettingsData => Addressables.LoadAssetAsync<T>(address);

		/// <summary>
		/// Indicates the status of whether
		/// this manager grabbed an instance of 
		/// <typeparamref name="TData"/>.
		/// </summary>
		public enum Status
		{
			/// <summary>
			/// Working on loading the project settings.
			/// </summary>
			Loading,
			/// <summary>
			/// Successfully loaded the project settings.
			/// </summary>
			Success,
			/// <summary>
			/// Failed to load the project settings,
			/// using default settings instead.
			/// </summary>
			UsingDefaultData,
			/// <summary>
			/// Failed to load the project settings,
			/// please try again.
			/// </summary>
			Fail
		}
	}
}
