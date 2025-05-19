#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Initializer
{
    static Initializer() 
    {
        EditorApplication.delayCall += () => 
        {
            // UIListStore.Instanceにアクセスすることで初期化をトリガー
            if (UIListStore.Instance != null)
            {
                Debug.Log("UIListStore initialization triggered on editor load.");
            }
            else
            {
                Debug.LogError("Failed to get UIListStore.Instance.");
            }           
        };
    }
}
#endif