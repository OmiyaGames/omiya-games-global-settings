using UnityEngine.UIElements;

namespace OmiyaGames.Global.Settings.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SettingsEditorHelpers.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/8/2022<br/>
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
	/// Helper methods for <seealso cref="BaseSettingsEditor{TData}"/>.
	/// </summary>
	public static class SettingsEditorHelpers
	{
		public const string OMIYA_GAMES_GROUP_NAME = "Omiya Games - Global Settings";
		/// <summary>
		/// Recursively hides elements who's label does not
		/// match the string in <paramref name="searchContext"/>.
		/// </summary>
		/// <param name="element">
		/// Element to hide.
		/// </param>
		/// <param name="searchContext">
		/// The label text to search for.
		/// </param>
		/// <returns>
		/// True if <paramref name="element"/> is still visible;
		/// false, otherwise.
		/// </returns>
		//public static bool UpdateElementVisibility(VisualElement element, string searchContext)
		//{
		//	// Check if this element is a label
		//	bool isVisible = false;
		//	if (element is Label)
		//	{
		//		// Compare the text in the label to search term
		//		string labelText = ((Label)element).text;

		//		// If there's a match, make this element visible
		//		isVisible |= searchContext.Equals(labelText, System.StringComparison.CurrentCultureIgnoreCase);
		//	}

		//	// If the label is visible, skip the children (thus, leaving them visible.)
		//	// Otherwise, go through the children and see if they are visible.
		//	if ((isVisible == false) && (element.childCount > 0))
		//	{
		//		foreach (var child in element.Children())
		//		{
		//			isVisible |= UpdateElementVisibility(child, searchContext);
		//		}
		//	}

		//	// Update the element's visibility
		//	element.visible = isVisible;
		//	return isVisible;
		//}
	}
}
