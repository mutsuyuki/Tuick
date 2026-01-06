using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tuick.Core
{
	public class USSList : ScriptableObject
	{
		[SerializeField] private StyleSheet[] ussList;

		private Dictionary<string, StyleSheet> _ussCache;

		private void OnEnable()
		{
			_ussCache = null;
		}

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

			if (_ussCache == null)
			{
				_ussCache = new Dictionary<string, StyleSheet>();
				if (ussList != null)
				{
					foreach (var sheet in ussList)
					{
						if (sheet != null && !_ussCache.ContainsKey(sheet.name))
						{
							_ussCache[sheet.name] = sheet;
						}
					}
				}
			}

			if (_ussCache.TryGetValue(className, out var styleSheet))
			{
				return styleSheet;
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
