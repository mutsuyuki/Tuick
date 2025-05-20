using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace Tuick.Core
{
	[CreateAssetMenu(menuName = "UI/List/USSList", fileName = "USSList")]
	public class USSList : ScriptableObject
	{
		[SerializeField] private StyleSheet[] ussList;

		public StyleSheet[] GetUxmlList()
		{
			return ussList;
		}

		public StyleSheet GetTemplate(string fullTypeName)
		{
			// Extract the class name from the fully qualified name (e.g., "Tuick.MyComponent" -> "MyComponent")
			string className = fullTypeName.Contains(".")
				? fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1)
				: fullTypeName;

			for (int i = 0; i < ussList.Length; i++)
			{
				if (ussList[i].name == className)
				{
					return ussList[i];
				}
			}

			return null;
		}

#if UNITY_EDITOR
		[ContextMenu("Load uss assets")]
		public void LoadUSSAssets()
		{
			LoadUSSAssets(PathUtil.GetUSSDirPath());
		}

 	public void LoadUSSAssets(string directoryPath)
		{
			List<string> paths = PathUtil.SearchDeployedUSSPaths();
			List<StyleSheet> validStyles = new List<StyleSheet>();

			for (int i = 0; i < paths.Count; i++)
			{
				StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(paths[i]);
				if (styleSheet != null)
				{
					validStyles.Add(styleSheet);
				}
				else
				{
					Debug.LogWarning($"Failed to load USS asset at path: {paths[i]}. It may have been deleted.");
				}
			}

			ussList = validStyles.ToArray();

			if (paths.Count > 0 && validStyles.Count == 0)
			{
				Debug.LogWarning("No valid USS assets found in the deployed directory.");
			}
			else if (paths.Count > validStyles.Count)
			{
				Debug.Log($"Filtered out {paths.Count - validStyles.Count} invalid USS assets.");
			}
		}
#endif
	}
}
