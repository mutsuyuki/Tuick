#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class PathUtil : AssetPostprocessor
{
    public static string GetLibRootPath()
    {
        // ライブラリルートのパスを取得(__ROOT__ファイルを探す)
        string[] assetGuids = AssetDatabase.FindAssets("__ROOT__");

        if (assetGuids.Length > 1)
        {
            Debug.LogError("Multiple Util scripts found.");
            return "";
        }

        string rootFilePath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
        return Path.GetDirectoryName(rootFilePath);
    }

    public static string GetTemplateDirPath()
    {
        string tempDirectory = Path.Combine(GetLibRootPath(), "Template");

        // Tempフォルダが存在しなければ作成
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        return tempDirectory;
    }

    public static string GetBuildDirPath()
    {
        string buildDirectory = Path.Combine(GetLibRootPath(), "Build");

        // Tempフォルダが存在しなければ作成
        if (!Directory.Exists(buildDirectory))
        {
            Directory.CreateDirectory(buildDirectory);
        }

        return buildDirectory;
    }

    public static string GetResourcesDirPath()
    {
        string resourcesDirectory = Path.Combine(GetBuildDirPath(), "Resources");

        // Resourcesフォルダが存在しなければ作成
        if (!Directory.Exists(resourcesDirectory))
        {
            Directory.CreateDirectory(resourcesDirectory);
        }

        return resourcesDirectory;
    }

    public static string GetUXMLDirPath()
    {
        string ussDirectory = Path.Combine(GetBuildDirPath(), "uxml");

        // Resourcesフォルダが存在しなければ作成
        if (!Directory.Exists(ussDirectory))
        {
            Directory.CreateDirectory(ussDirectory);
        }

        return ussDirectory;
    }

    public static string GetUSSDirPath()
    {
        string ussDirectory = Path.Combine(GetBuildDirPath(), "uss");

        // Resourcesフォルダが存在しなければ作成
        if (!Directory.Exists(ussDirectory))
        {
            Directory.CreateDirectory(ussDirectory);
        }

        return ussDirectory;
    }

    public static List<string> SearchSourceUXMLPaths(string directoryPath = "Assets")
    {
        string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { directoryPath });
        List<string> paths = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (
                path.Contains(GetUXMLDirPath()) ||
                path.Contains(GetTemplateDirPath())
            )
            {
                continue;
            }

            paths.Add(path);
        }

        return paths;
    }

    public static List<string> SearchSourceUSSPaths(string directoryPath = "Assets")
    {
        string[] guids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { directoryPath });
        List<string> paths = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (
                path.Contains(GetUSSDirPath()) ||
                path.Contains(GetTemplateDirPath()) ||
                !path.EndsWith(".uss"))
            {
                continue;
            }

            paths.Add(path);
        }

        return paths;
    }

    public static List<string> SearchDeployedUXMLPaths()
    {
        string directoryPath = GetUXMLDirPath();
        string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { directoryPath });
        List<string> paths = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            paths.Add(path);
        }

        return paths;
    }



    public static List<string> SearchDeployedUSSPaths()
    {
        string directoryPath = GetUSSDirPath();
        string[] guids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { directoryPath });
        List<string> paths = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            paths.Add(path);
        }

        return paths;
    }
}

#endif