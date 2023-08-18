using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "UXMLList", fileName = "UXMLList")]
public class UXMLList : ScriptableObject
{
    [SerializeField] private VisualTreeAsset[] uxmlList;

    public VisualTreeAsset[] GetUxmlList()
    {
        return uxmlList;
    }

    public TemplateContainer GetTemplate(string name)
    {
        for (int i = 0; i < uxmlList.Length; i++)
        {
            if (uxmlList[i].name == name)
            {
                return uxmlList[i].CloneTree();
            }
        }
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Load uxml assets")]
    public void LoadUXMLAssets()
    {
        LoadUXMLAssets(PathUtil.GetUXMLDirPath());
    }

    public void LoadUXMLAssets(string directoryPath)
    {
        List<string> paths = PathUtil.SearchDeployedUXMLPaths();
        uxmlList = new VisualTreeAsset[paths.Count];
        for (int i = 0; i < paths.Count; i++)
        {
            Debug.Log("uxml:" + i.ToString() + " " + paths[i]);
            uxmlList[i] = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(paths[i]);
        }
    }
#endif
}