#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Tuick.Core
{
	public class PathUtil : AssetPostprocessor
	{
		// ライブラリのルートパスのキャッシュ
		private static string _cachedLibRootPath = null;

		// プロジェクトのアセット内に作成されるTuick関連フォルダの定義
		private const string PROJECT_ASSETS_TUICK_ROOT_FOLDER_NAME = "Tuick";
		private const string BUILD_FOLDER_NAME_IN_ASSETS = "Build";
		private const string GITIGNORE_FILE_NAME = ".gitignore";
		private const string GITIGNORE_CONTENT_FOR_BUILD_DIR = "*";

		// Assembly Definition Fileをライブラリのルートパスとする
		private const string FrameworkAssemblyName = "Tuick";

		public static string GetLibRootPath()
		{
			// キャッシュされたパスがあればそれを返す
			if (_cachedLibRootPath != null)
			{
				return _cachedLibRootPath;
			}

			// Assembly Definition File (.asmdef) を基準にする
			string[] guids = AssetDatabase.FindAssets($"t:AssemblyDefinitionAsset {FrameworkAssemblyName}");

			if (guids.Length == 0)
			{
				Debug.LogError(
					$"PathUtil: Could not find the Assembly Definition File where the 'Name' field is set to '{FrameworkAssemblyName}'. " +
					$"This name should match the 'Name' field in your .asmdef file (e.g., {FrameworkAssemblyName}.asmdef). " +
					$"Cannot determine library root path.");
				_cachedLibRootPath = string.Empty; // エラー時は空文字列をキャッシュ
				return _cachedLibRootPath;
			}

			if (guids.Length > 1)
			{
				string foundPaths = "";
				foreach (var guid in guids)
				{
					foundPaths += AssetDatabase.GUIDToAssetPath(guid) + "\n";
				}

				Debug.LogWarning(
					$"PathUtil: Multiple Assembly Definition Files found where the 'Name' field is '{FrameworkAssemblyName}'. " +
					$"Using the first one found: {AssetDatabase.GUIDToAssetPath(guids[0])}\nFound paths:\n{foundPaths}");
			}

			string asmdefPath = AssetDatabase.GUIDToAssetPath(guids[0]);
			_cachedLibRootPath = Path.GetDirectoryName(asmdefPath);

			return _cachedLibRootPath;
		}

		// Assets/Tuick フォルダのパスを取得（なければ作成）
		public static string GetTuickProjectAssetsPath()
		{
			string assetsPath = "Assets";
			string tuickAssetsPath = Path.Combine(assetsPath, PROJECT_ASSETS_TUICK_ROOT_FOLDER_NAME);

			try
			{
				if (!Directory.Exists(tuickAssetsPath))
				{
					Directory.CreateDirectory(tuickAssetsPath);
					AssetDatabase.Refresh(); // Unityエディタに新しいフォルダを認識させる
					Debug.Log($"PathUtil: Created project assets Tuick directory: {tuickAssetsPath}");
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"PathUtil: Failed to create project assets Tuick directory at '{tuickAssetsPath}'. Error: {e.Message}");
				return string.Empty;
			}
			return tuickAssetsPath;
		}

		private static void EnsureGitignoreInBuildDir(string buildDirPath)
		{
			string gitignorePath = Path.Combine(buildDirPath, GITIGNORE_FILE_NAME);
			if (!File.Exists(gitignorePath))
			{
				try
				{
					File.WriteAllText(gitignorePath, GITIGNORE_CONTENT_FOR_BUILD_DIR);
					AssetDatabase.ImportAsset(gitignorePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
					Debug.Log($"PathUtil: Created .gitignore in Build directory: {gitignorePath}");
				}
				catch (System.Exception e)
				{
					Debug.LogError($"PathUtil: Failed to create .gitignore in '{buildDirPath}'. Error: {e.Message}");
				}
			}
		}

		public static string GetTemplateDirPath()
		{
			string libRoot = GetLibRootPath();
			if (string.IsNullOrEmpty(libRoot))
			{
				Debug.LogWarning("PathUtil: Cannot get Template directory path. Library root path is invalid.");
				return string.Empty;
			}

			string tempDirectory = Path.Combine(libRoot, "Templates");

			// Tempフォルダが存在しなければ作成
			try
			{
				if (!Directory.Exists(tempDirectory))
				{
					Directory.CreateDirectory(tempDirectory);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError(
					$"PathUtil: Failed to create Template directory at '{tempDirectory}'. Error: {e.Message}");
				return string.Empty;
			}

			return tempDirectory;
		}

		public static string GetBuildDirPath()
		{
			string tuickAssetsRoot = GetTuickProjectAssetsPath();
			if (string.IsNullOrEmpty(tuickAssetsRoot)) 
			{
				Debug.LogWarning("PathUtil: Cannot get Build directory path. Tuick project assets path is invalid.");
				return string.Empty;
			}
			string buildDirectory = Path.Combine(tuickAssetsRoot, BUILD_FOLDER_NAME_IN_ASSETS);

			try
			{
				if (!Directory.Exists(buildDirectory))
				{
					Directory.CreateDirectory(buildDirectory);
				}
			}
			catch (IOException e)
			{
				Debug.LogError($"PathUtil: IO Error creating Build directory at '{buildDirectory}'. Error: {e.Message}. Check permissions or path validity.");
				return string.Empty;
			}
			EnsureGitignoreInBuildDir(buildDirectory);
			return buildDirectory;
		}

		public static string GetResourcesDirPath()
		{
			string buildDir = GetBuildDirPath();
			if (string.IsNullOrEmpty(buildDir))
			{
				Debug.LogWarning("PathUtil: Cannot get Resources directory path. Build directory path is invalid.");
				return string.Empty;
			}

			string resourcesDirectory = Path.Combine(buildDir, "Resources");

			// Resourcesフォルダが存在しなければ作成
			try
			{
				if (!Directory.Exists(resourcesDirectory))
				{
					Directory.CreateDirectory(resourcesDirectory);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError(
					$"PathUtil: Failed to create Resources directory at '{resourcesDirectory}'. Error: {e.Message}");
				return string.Empty;
			}

			return resourcesDirectory;
		}

		public static string GetUXMLDirPath()
		{
			string buildDir = GetBuildDirPath();
			if (string.IsNullOrEmpty(buildDir))
			{
				Debug.LogWarning("PathUtil: Cannot get UXML directory path. Build directory path is invalid.");
				return string.Empty;
			}

			// 元のコードで変数名がussDirectoryになっていたのを修正
			string uxmlDirectory = Path.Combine(buildDir, "uxml");

			// UXMLフォルダが存在しなければ作成
			try
			{
				if (!Directory.Exists(uxmlDirectory))
				{
					Directory.CreateDirectory(uxmlDirectory);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"PathUtil: Failed to create UXML directory at '{uxmlDirectory}'. Error: {e.Message}");
				return string.Empty;
			}

			return uxmlDirectory;
		}

		public static string GetUSSDirPath()
		{
			string buildDir = GetBuildDirPath();
			if (string.IsNullOrEmpty(buildDir))
			{
				Debug.LogWarning("PathUtil: Cannot get USS directory path. Build directory path is invalid.");
				return string.Empty;
			}

			string ussDirectory = Path.Combine(buildDir, "uss");

			// USSフォルダが存在しなければ作成
			try
			{
				if (!Directory.Exists(ussDirectory))
				{
					Directory.CreateDirectory(ussDirectory);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"PathUtil: Failed to create USS directory at '{ussDirectory}'. Error: {e.Message}");
				return string.Empty;
			}

			return ussDirectory;
		}

		public static List<string> SearchSourceUXMLPaths(string directoryPath = "Assets")
		{
			string uxmlBuildPath = GetUXMLDirPath();
			string templatePath = GetTemplateDirPath();

			// ビルドパスまたはテンプレートパスの取得に失敗した場合は処理を中断
			if (string.IsNullOrEmpty(uxmlBuildPath) || string.IsNullOrEmpty(templatePath))
			{
				Debug.LogWarning(
					"PathUtil: Cannot search source UXML paths. UXML build path or Template path is invalid.");
				return new List<string>();
			}

			List<string> paths = new List<string>();
			string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { directoryPath });
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				// ビルド出力先 (Assets/Tuick/Build/uxml) とライブラリ内のテンプレートを除外
				if (path.StartsWith(uxmlBuildPath) || path.StartsWith(templatePath))
				{
					continue;
				}

				// Check for corresponding C# file
				string csPath = Path.ChangeExtension(path, ".cs");
				if (File.Exists(csPath))
				{
					paths.Add(path);
				}
			}

			return paths;
		}

		public static List<string> SearchSourceUSSPaths(string directoryPath = "Assets")
		{
			string ussBuildPath = GetUSSDirPath();
			string templatePath = GetTemplateDirPath();

			if (string.IsNullOrEmpty(ussBuildPath) || string.IsNullOrEmpty(templatePath))
			{
				Debug.LogWarning(
					"PathUtil: Cannot search source USS paths. USS build path or Template path is invalid.");
				return new List<string>();
			}

			List<string> paths = new List<string>();
			string[] guids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { directoryPath });
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				if (!path.EndsWith(".uss") ||
				    path.StartsWith(ussBuildPath) ||
				    path.StartsWith(templatePath))
				{
					continue;
				}

				// Check for corresponding C# file
				string csPath = Path.ChangeExtension(path, ".cs");
				if (File.Exists(csPath))
				{
					paths.Add(path);
				}
			}

			return paths;
		}

		public static List<string> SearchDeployedUXMLPaths()
		{
			string deployedUxmlDir = GetUXMLDirPath();
			if (string.IsNullOrEmpty(deployedUxmlDir))
			{
				Debug.LogWarning(
					"PathUtil: Cannot search deployed UXML paths. Deployed UXML directory path is invalid.");
				return new List<string>();
			}

			List<string> paths = new List<string>();
			string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { deployedUxmlDir });
			for (int i = 0; i < guids.Length; i++)
			{
				paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
			}

			return paths;
		}

		public static List<string> SearchDeployedUSSPaths()
		{
			string deployedUssDir = GetUSSDirPath();
			if (string.IsNullOrEmpty(deployedUssDir))
			{
				Debug.LogWarning("PathUtil: Cannot search deployed USS paths. Deployed USS directory path is invalid.");
				return new List<string>();
			}

			List<string> paths = new List<string>();
			string[] guids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { deployedUssDir });
			for (int i = 0; i < guids.Length; i++)
			{
				paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
			}

			return paths;
		}
	}
}

#endif
