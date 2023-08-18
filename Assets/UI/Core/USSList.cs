using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "USSList", fileName = "USSList")]
public class USSList : ScriptableObject
{
    [SerializeField] private StyleSheet[] ussList;

    public StyleSheet[] GetUxmlList()
    {
        return ussList;
    }

    public StyleSheet GetTemplate(string name)
    {
        for (int i = 0; i < ussList.Length; i++)
        {
            if (ussList[i].name == name)
            {
                return ussList[i];
            }
        }

        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Load uss assets")]
    public void LoadUSSAssets()
    {
        LoadUSSAssets(PathUtil.GetUSSDirPath());
    }

    public void LoadUSSAssets(string directoryPath)
    {
        List<string> paths = PathUtil.SearchDeployedUSSPaths();
        ussList = new StyleSheet[paths.Count];
        for (int i = 0; i < paths.Count; i++)
        {
            Debug.Log("uss:" + i.ToString() + " " + paths[i]);
            ussList[i] = AssetDatabase.LoadAssetAtPath<StyleSheet>(paths[i]);
            
        }
    }
#endif
}