#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


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
                if (path.Contains(PathUtil.GetUXMLDirPath()))
                {
                    continue;
                }

                UXMLProcessor.Deploy(path);
                isUXMLChanged = true;
            }

            if (path.EndsWith(".uss"))
            {
                if (path.Contains(PathUtil.GetUSSDirPath()))
                {
                    continue;
                }

                USSProcessor.Deploy(path);
                isUSSChanged = true;
            }
        }

        if (isUXMLChanged)
        {
            UIListStore.Instance.GetUXMLList().LoadUXMLAssets();
        }

        if (isUSSChanged)
        {
            UIListStore.Instance.GetUSSList().LoadUSSAssets();
        }
    }
}

#endif