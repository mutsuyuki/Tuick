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

    public static string GetTempDirPath()
    {
        string tempDirectory = Path.Combine(GetLibRootPath(), "Temp");

        // Tempフォルダが存在しなければ作成
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        return tempDirectory;
    }

    public static string GetResourcesDirPath()
    {
        string resourcesDirectory = Path.Combine(GetTempDirPath(), "Resources");

        // Resourcesフォルダが存在しなければ作成
        if (!Directory.Exists(resourcesDirectory))
        {
            Directory.CreateDirectory(resourcesDirectory);
        }

        return resourcesDirectory;
    }

    public static string GetUXMLDirPath()
    {
        string ussDirectory = Path.Combine(GetTempDirPath(), "uxml");

        // Resourcesフォルダが存在しなければ作成
        if (!Directory.Exists(ussDirectory))
        {
            Directory.CreateDirectory(ussDirectory);
        }

        return ussDirectory;
    }

    public static string GetUSSDirPath()
    {
        string ussDirectory = Path.Combine(GetTempDirPath(), "uss");

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
            if (path.Contains(GetUXMLDirPath()))
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


    public static List<string> SearchUSSPaths(string directoryPath = "Assets")
    {
        string[] guids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { directoryPath });
        List<string> paths = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (path.Contains(GetUSSDirPath()) || !path.EndsWith(".uss"))
            {
                continue;
            }

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