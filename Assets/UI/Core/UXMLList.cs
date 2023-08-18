using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
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
    [ContextMenu("Load all uxml assets")]
    public void LoadUXMLAssets()
    {
        LoadUXMLAssets("Assets");
    }

    public void LoadUXMLAssets(string directoryPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { directoryPath });
        uxmlList = new VisualTreeAsset[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            uxmlList[i] = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
        }
    }
#endif
}