#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class PathUtil : AssetPostprocessor
{
    // ライブラリのルートパスのキャッシュ
    private static string _cachedLibRootPath = null;

    // Assembly Definition Fileをルートパスとする
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
            Debug.LogError($"PathUtil: Could not find the Assembly Definition File where the 'Name' field is set to '{FrameworkAssemblyName}'. " +
                           $"This name should match the 'Name' field in your .asmdef file (e.g., {FrameworkAssemblyName}.asmdef). " +
                           $"Cannot determine library root path.");
            _cachedLibRootPath = string.Empty; // エラー時は空文字列をキャッシュ
            return _cachedLibRootPath;
        }

        if (guids.Length > 1)
        {
            string foundPaths = "";
            foreach(var guid in guids) {
                foundPaths += AssetDatabase.GUIDToAssetPath(guid) + "\n";
            }
            Debug.LogWarning($"PathUtil: Multiple Assembly Definition Files found where the 'Name' field is '{FrameworkAssemblyName}'. " +
                             $"Using the first one found: {AssetDatabase.GUIDToAssetPath(guids[0])}\nFound paths:\n{foundPaths}");
        }

        string asmdefPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        _cachedLibRootPath = Path.GetDirectoryName(asmdefPath);

        return _cachedLibRootPath;
    }

    public static string GetTemplateDirPath()
    {
        string libRoot = GetLibRootPath();
        if (string.IsNullOrEmpty(libRoot))
        {
            Debug.LogWarning("PathUtil: Cannot get Template directory path. Library root path is invalid.");
            return string.Empty;
        }

        string tempDirectory = Path.Combine(libRoot, "Template");

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
            Debug.LogError($"PathUtil: Failed to create Template directory at '{tempDirectory}'. Error: {e.Message}");
            return string.Empty;
        }

        return tempDirectory;
    }

    public static string GetBuildDirPath()
    {
        string libRoot = GetLibRootPath();
        if (string.IsNullOrEmpty(libRoot))
        {
            Debug.LogWarning("PathUtil: Cannot get Build directory path. Library root path is invalid.");
            return string.Empty;
        }

        string buildDirectory = Path.Combine(libRoot, "Build");

        // Buildフォルダが存在しなければ作成
        try
        {
            if (!Directory.Exists(buildDirectory))
            {
                Directory.CreateDirectory(buildDirectory);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PathUtil: Failed to create Build directory at '{buildDirectory}'. Error: {e.Message}");
            return string.Empty;
        }

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
            Debug.LogError($"PathUtil: Failed to create Resources directory at '{resourcesDirectory}'. Error: {e.Message}");
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

        // 必要なディレクトリパスが取得できない場合は空のリストを返す
        if (string.IsNullOrEmpty(uxmlBuildPath) || string.IsNullOrEmpty(templatePath))
        {
            Debug.LogWarning("PathUtil: Cannot search source UXML paths. UXML build path or Template path is invalid.");
            return new List<string>();
        }

        List<string> paths = new List<string>();
        string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { directoryPath });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            // ビルド先やテンプレートディレクトリ内のファイルは除外
            if (path.Contains(uxmlBuildPath) || path.Contains(templatePath))
            {
                continue;
            }
            paths.Add(path);
        }
        return paths;
    }

    public static List<string> SearchSourceUSSPaths(string directoryPath = "Assets")
    {
        string ussBuildPath = GetUSSDirPath();
        string templatePath = GetTemplateDirPath();

        if (string.IsNullOrEmpty(ussBuildPath) || string.IsNullOrEmpty(templatePath))
        {
            Debug.LogWarning("PathUtil: Cannot search source USS paths. USS build path or Template path is invalid.");
            return new List<string>();
        }

        List<string> paths = new List<string>();
        string[] guids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { directoryPath });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (!path.EndsWith(".uss") ||
                path.Contains(ussBuildPath) ||
                path.Contains(templatePath))
            {
                continue;
            }
            paths.Add(path);
        }
        return paths;
    }

    public static List<string> SearchDeployedUXMLPaths()
    {
        string deployedUxmlDir = GetUXMLDirPath();
        if (string.IsNullOrEmpty(deployedUxmlDir))
        {
            Debug.LogWarning("PathUtil: Cannot search deployed UXML paths. Deployed UXML directory path is invalid.");
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

#endif