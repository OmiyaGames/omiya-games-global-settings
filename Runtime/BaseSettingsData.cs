using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Global.Settings
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="BaseSettingsData.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/5/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Initial version.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// The data stored as addressables that the editor and runtime references
	/// </summary>
	public abstract class BaseSettingsData : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		int serializedVersion = 0;

		/// <summary>
		/// The current version of this setting.
		/// </summary>
		/// <remarks>
		/// Always, ALWAYS update this whenever an upgrade from
		/// an old version of the base settings needs to be updated.
		/// </remarks>
		public abstract int CurrentVersion
		{
			get;
		}

		public virtual void OnAfterDeserialize()
		{
			// Do nothing
		}

		public virtual void OnBeforeSerialize()
		{
			// Check if upgrade is necessary; if so, run the event.
			if ((serializedVersion < CurrentVersion) && (OnUpgrade(serializedVersion, out string errorMessage) == false))
			{
				// Log any errors in the process
				Debug.LogError(errorMessage, this);
			}
		}

		/// <summary>
		/// This event is called when the serialized object is older than <see cref="CurrentVersion"/>.
		/// </summary>
		/// <param name="oldVersion">The last serialization version number.</param>
		/// <param name="errorMessage">Error message to print on failure.</param>
		/// <returns>True if successful; false, otherwise.</returns>
		protected virtual bool OnUpgrade(int oldVersion, out string errorMessage)
		{
			errorMessage = null;
			return true;
		}
	}
}
