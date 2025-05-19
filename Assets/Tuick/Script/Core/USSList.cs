using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace Tuick
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
			ussList = new StyleSheet[paths.Count];
			for (int i = 0; i < paths.Count; i++)
			{
				ussList[i] = AssetDatabase.LoadAssetAtPath<StyleSheet>(paths[i]);
			}
		}
#endif
	}
}