#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;


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

        if (isUXMLChanged)
        {
            try
            {
                UIListStore.Instance.GetUXMLList().LoadUXMLAssets();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        if (isUSSChanged)
        {
            try
            {
                UIListStore.Instance.GetUSSList().LoadUSSAssets();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}

#endif