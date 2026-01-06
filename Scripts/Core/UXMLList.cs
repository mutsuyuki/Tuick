using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tuick.Core
{
	public class UXMLList : ScriptableObject
	{
		[SerializeField] private VisualTreeAsset[] uxmlList;

		private Dictionary<string, VisualTreeAsset> _uxmlCache;

		private void OnEnable()
		{
			_uxmlCache = null;
		}

		public VisualTreeAsset[] GetUxmlList()
		{
			return uxmlList;
		}

		public TemplateContainer GetTemplate(string fullTypeName)
		{
			// Extract the class name from the fully qualified name (e.g., "Tuick.MyComponent" -> "MyComponent")
			string className = fullTypeName.Contains(".")
				? fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1)
				: fullTypeName;

			if (_uxmlCache == null)
			{
				_uxmlCache = new Dictionary<string, VisualTreeAsset>();
				if (uxmlList != null)
				{
					foreach (var asset in uxmlList)
					{
						if (asset != null && !_uxmlCache.ContainsKey(asset.name))
						{
							_uxmlCache[asset.name] = asset;
						}
					}
				}
			}

			if (_uxmlCache.TryGetValue(className, out var template))
			{
				return template.CloneTree();
			}

			return null;
		}

#if UNITY_EDITOR
		[ContextMenu("Load uxml assets")]
		public void LoadUXMLAssets()
		{
			LoadUXMLAssets(PathUtil.GetUXMLDirPath());
		}

 	public void LoadUXMLAssets(string directoryPath)
		{
			List<string> paths = PathUtil.SearchDeployedUXMLPaths();
			List<VisualTreeAsset> validAssets = new List<VisualTreeAsset>();

			for (int i = 0; i < paths.Count; i++)
			{
				VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(paths[i]);
				if (asset != null)
				{
					validAssets.Add(asset);
				}
				else
				{
					Debug.LogWarning($"Failed to load UXML asset at path: {paths[i]}. It may have been deleted.");
				}
			}

			uxmlList = validAssets.ToArray();

			if (paths.Count > 0 && validAssets.Count == 0)
			{
				Debug.LogWarning("No valid UXML assets found in the deployed directory.");
			}
			else if (paths.Count > validAssets.Count)
			{
				Debug.Log($"Filtered out {paths.Count - validAssets.Count} invalid UXML assets.");
			}
		}
#endif
	}
}
