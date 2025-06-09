#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tuick.Core
{
	public class CreateComponent
	{
		[MenuItem("Assets/Create/Tuick/Component", false, 50)]
 	public static void CreateComponentMenu()
 	{
 		ComponentNameInput.Show(
 			new Vector2(100, 100),
 			"",
 			x => { },
 			(name, createFolder) => CopyTemplates(name, createFolder),
 			"Enter the name for\nthe new UI Component.",
 			300
 		);
 	}

 	private static void CopyTemplates(string name, bool createFolder)
		{
			if (string.IsNullOrEmpty(name))
				return;

			string srcPath = PathUtil.GetLibRootPath();
			if (string.IsNullOrEmpty(srcPath))
			{
				Debug.LogError("CreateComponent: Could not determine library root path. Component creation aborted.");
				return;
			}

			srcPath = Path.Combine(srcPath, "Templates");

 		string distPath = AssetDatabase.GetAssetPath(Selection.activeObject);
 		if (string.IsNullOrEmpty(distPath))
 		{
 			distPath = "Assets";
 		}
 		else if (Path.GetExtension(distPath) != "")
 		{
 			distPath = Path.GetDirectoryName(distPath);
 		}

 		// name最初の１文字を大文字に
 		string nameUpper = char.ToUpper(name[0]) + name.Substring(1);

 		// If createFolder is true, create a subdirectory with the same name as the component
 		string finalDistPath = distPath;
 		if (createFolder)
 		{
 			finalDistPath = Path.Combine(distPath, nameUpper);
 			if (!Directory.Exists(finalDistPath))
 			{
 				Directory.CreateDirectory(finalDistPath);
 				AssetDatabase.Refresh(); // Refresh AssetDatabase to recognize the new directory
 			}
 		}

			// ビルドプロセスを開始
			AssetDatabase.StartAssetEditing();
			try
			{
				// uxmlファイルはコピーのみ
				string srcUXMLPath = Path.Combine(srcPath, "ComponentTemplate.uxml");
				string distUXMLPath = Path.Combine(finalDistPath, nameUpper + ".uxml");
				if (File.Exists(srcUXMLPath))
				{
					File.Copy(srcUXMLPath, distUXMLPath, false);
					AssetDatabase.ImportAsset(distUXMLPath,
						ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
				}
				else
				{
					Debug.LogError($"CreateComponent: ComponentTemplate UXML file not found at {srcUXMLPath}");
				}

				// ussファイルは名前を使って内容書き換え
				string srcUSSPath = Path.Combine(srcPath, "ComponentTemplate.uss");
				string distUSSPath = Path.Combine(finalDistPath, nameUpper + ".uss");
				if (File.Exists(srcUSSPath))
				{
					string contentUSS = File.ReadAllText(srcUSSPath);
					contentUSS = contentUSS.Replace("ComponentTemplate", nameUpper);
					File.WriteAllText(distUSSPath, contentUSS);
					AssetDatabase.ImportAsset(distUSSPath,
						ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
				}
				else
				{
					Debug.LogError($"CreateComponent: ComponentTemplate USS file not found at {srcUSSPath}");
				}

				// csファイルはクラス名を書き換え
				string srcCSharpPath = Path.Combine(srcPath, "ComponentTemplate.cs");
				string distCSharpPath = Path.Combine(finalDistPath, nameUpper + ".cs");
				if (File.Exists(srcCSharpPath))
				{
					string contentCSharp = File.ReadAllText(srcCSharpPath);
					contentCSharp = contentCSharp.Replace("ComponentTemplate", nameUpper);
					File.WriteAllText(distCSharpPath, contentCSharp);
					AssetDatabase.ImportAsset(distCSharpPath,
						ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
				}
				else
				{
					Debug.LogError($"CreateComponent: ComponentTemplate C# file not found at {srcCSharpPath}");
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"CreateComponent: Error during template copying for '{nameUpper}'. Exception: {e}");
			}
			finally
			{
				AssetDatabase.StopAssetEditing(); // アセット操作を終了し、インポートをトリガー
			}

			// uxml,ussのリストを更新 (次のエディタフレームで実行)
			EditorApplication.delayCall += () =>
			{
				UIListStore.Instance.Refresh();
				Debug.Log($"CreateComponent: Updating UXML/USS lists after creating '{nameUpper}'.");
			};
		}
	}
}

#endif
