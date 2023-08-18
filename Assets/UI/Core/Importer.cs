using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;


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
        foreach (string path in importedAssets.Concat(deletedAssets))
        {
            if (path.EndsWith(".uxml"))
            {
                isUXMLChanged = true;
            }

            if (path.EndsWith(".uss"))
            {
                ProcessUSS(path);
                isUSSChanged = true;
            }
        }

        if (isUXMLChanged)
        {
            UIListStore.Instance.GetUXMLList().LoadUXMLAssets("Assets");
        }

        if (isUSSChanged)
        {

        }
    }

    private static void ProcessUSS(string ussPath)
    {
        // ussを編集
        string ussContent = File.ReadAllText(ussPath);
        ussContent = ussContent.Replace("\n", "").Replace("\r", "");

        // 編集後ussファイルを保存
        string saveDir = Path.Combine(GetTempDirectoryPath(), "uss");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        string savePath = Path.Combine(GetTempDirectoryPath(), Path.GetFileName(ussPath));
        File.WriteAllText(savePath, ussContent);
    }

    private static string GetTempDirectoryPath()
    {
        // Tempフォルダのパスを計算
        string scriptPath = GetScriptPath();
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        string tempDirectory = Path.Combine(scriptDirectory, "Temp");

        // Tempフォルダが存在しなければ作成
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        return tempDirectory;
    }

    private static string GetScriptPath()
    {
        // 本スクリプトの場所を取得
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        string[] assetGuids = AssetDatabase.FindAssets(className + " t:script");
        if (assetGuids.Length > 1)
        {
            Debug.LogError("Multiple USSImport scripts found.");
            return "";
        }

        return AssetDatabase.GUIDToAssetPath(assetGuids[0]);
    }
}