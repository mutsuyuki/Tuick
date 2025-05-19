#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tuick
{
	public class Importer : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths
		)
		{
			bool isUXMLChanged = false;
			bool isUSSChanged = false;

			// Process imported assets
			foreach (string path in importedAssets)
			{
				if (path.EndsWith(".uxml"))
				{
					if (
						path.Contains(PathUtil.GetUXMLDirPath()) ||
						path.Contains(PathUtil.GetTemplateDirPath())
					)
					{
						continue;
					}

					UXMLProcessor.Deploy(path);
					isUXMLChanged = true;
				}

				if (path.EndsWith(".uss"))
				{
					if (
						path.Contains(PathUtil.GetUSSDirPath()) ||
						path.Contains(PathUtil.GetTemplateDirPath())
					)
					{
						continue;
					}

					USSProcessor.Deploy(path);
					isUSSChanged = true;
				}
			}

			// Process deleted assets
			bool isUXMLDeleted = false;
			bool isUSSDeleted = false;

			foreach (string deletedPath in deletedAssets)
			{
				// Skip files in build or template directories
				if (deletedPath.Contains(PathUtil.GetBuildDirPath()) || 
				    deletedPath.Contains(PathUtil.GetTemplateDirPath()))
				{
					continue;
				}

				string fileName = Path.GetFileName(deletedPath);

				if (deletedPath.EndsWith(".uxml"))
				{
					// Delete the corresponding file in the build directory
					string deployedPath = Path.Combine(PathUtil.GetUXMLDirPath(), fileName);
					if (File.Exists(deployedPath))
					{
						Debug.Log($"Deleting deployed UXML file: {deployedPath}");
						AssetDatabase.DeleteAsset(deployedPath);
						isUXMLDeleted = true;
					}
				}
				else if (deletedPath.EndsWith(".uss"))
				{
					// Delete the corresponding file in the build directory
					string deployedPath = Path.Combine(PathUtil.GetUSSDirPath(), fileName);
					if (File.Exists(deployedPath))
					{
						Debug.Log($"Deleting deployed USS file: {deployedPath}");
						AssetDatabase.DeleteAsset(deployedPath);
						isUSSDeleted = true;
					}
				}
			}

			// Update UXML list if needed
			if (isUXMLChanged || isUXMLDeleted)
			{
				try
				{
					UXMLList uxmlList = UIListStore.Instance.GetUXMLList();
					if (uxmlList != null)
					{
						uxmlList.LoadUXMLAssets();
						EditorUtility.SetDirty(uxmlList); // Mark as dirty to ensure changes are saved
					}
				}
				catch (Exception e)
				{
					Debug.LogError($"Error updating UXML list: {e.Message}");
				}
			}

			// Update USS list if needed
			if (isUSSChanged || isUSSDeleted)
			{
				try
				{
					USSList ussList = UIListStore.Instance.GetUSSList();
					if (ussList != null)
					{
						ussList.LoadUSSAssets();
						EditorUtility.SetDirty(ussList); // Mark as dirty to ensure changes are saved
					}
				}
				catch (Exception e)
				{
					Debug.LogError($"Error updating USS list: {e.Message}");
				}
			}

			// Refresh asset database if any changes were made
			if (isUXMLChanged || isUSSChanged || isUXMLDeleted || isUSSDeleted)
			{
				AssetDatabase.SaveAssets(); // Save any changes to ScriptableObjects
				AssetDatabase.Refresh();
			}
		}
	}
}

#endif
