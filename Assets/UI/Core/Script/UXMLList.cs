using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace Tuick
{
[CreateAssetMenu(menuName = "UI/List/UXMLList", fileName = "UXMLList")]
public class UXMLList : ScriptableObject
{
    [SerializeField] private VisualTreeAsset[] uxmlList;

    public VisualTreeAsset[] GetUxmlList()
    {
        return uxmlList;
    }

    public TemplateContainer GetTemplate(string fullTypeName)
    {
        // Extract the class name from the fully qualified name (e.g., "Tuick.MyComponent" -> "MyComponent")
        string className = fullTypeName.Contains(".") ? 
            fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1) : 
            fullTypeName;

        for (int i = 0; i < uxmlList.Length; i++)
        {
            if (uxmlList[i].name == className)
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
            uxmlList[i] = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(paths[i]);
        }
    }
#endif
}
}