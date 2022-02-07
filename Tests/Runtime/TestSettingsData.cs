using NUnit.Framework;

namespace OmiyaGames.Global.Settings.Tests
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="TestSettingsData.cs" company="Omiya Games">
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
	/// <description>Initial version.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Test instance of <seealso cref="BaseSettingsData"/>.
	/// </summary>
	public class TestSettingsData : BaseSettingsData
	{
		/// <summary>
		/// Allows one to change <see cref="CurrentVersion"/>.
		/// </summary>
		public static int TestVersion
		{
			get;
			set;
		} = 0;
		/// <summary>
		/// Set to true if we're expecting <see cref="OnUpgrade(int, out string)"/>
		/// to be called on deserialization.
		/// </summary>
		public static bool AsserOnUpgradeCalled
		{
			get;
			set;
		} = false;

		bool isOnUpgradeCalled = false;

		/// <inheritdoc/>
		public override int CurrentVersion => TestVersion;

		/// <inheritdoc/>
		public override void OnAfterDeserialize()
		{
			// Reset upgrade flag
			isOnUpgradeCalled = false;

			// Check if upgrade is necessary; if so, run the event.
			base.OnAfterDeserialize();

			// Assert whether OnUpgrade was called or not
			string message = (AsserOnUpgradeCalled ? "Expected OnUpgrade() will be called." : "Expected OnUpgrade() will NOT be called.");
			Assert.AreEqual(AsserOnUpgradeCalled, isOnUpgradeCalled, message);
		}

		/// <inheritdoc/>
		protected override bool OnUpgrade(int oldVersion, out string errorMessage)
		{
			// Flag that upgrade is being called
			isOnUpgradeCalled = true;
			return base.OnUpgrade(oldVersion, out errorMessage);
		}
	}
}
