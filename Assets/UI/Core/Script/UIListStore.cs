using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Tuick
{
public class UIListStore
{
    // uxmlリストデータ
    private UXMLList uxmlListData;

    public UXMLList GetUXMLList()
    {
        return uxmlListData;
    }

    // ussリストデータ
    private USSList ussListData;

    public USSList GetUSSList()
    {
        return ussListData;
    }

    // シングルトン化
    private UIListStore() {}
    private static UIListStore _instance;
    public static UIListStore Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIListStore();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    private void Initialize()
    {
#if UNITY_EDITOR
        Debug.Log("UIListStore: Initialization started.");

        // Resourcesフォルダのパスを取得
        string saveDir = PathUtil.GetResourcesDirPath();
        if (string.IsNullOrEmpty(saveDir))
        {
            Debug.LogError("UIListStore: Could not get Resources directory path. Initialization aborted.");
            return;
        }

        // umxl管理ファイルがない場合は作る
        string uxmlListPath = Path.Combine(saveDir, nameof(UXMLList)) + ".asset";
        UXMLList localUxmlListData = AssetDatabase.LoadAssetAtPath<UXMLList>(uxmlListPath);

        if (localUxmlListData == null)
        {
            localUxmlListData = UXMLList.CreateInstance<UXMLList>();

            try
            {
                // uxmlファイルをコピー
                UXMLProcessor.DeployAll();
                // 管理ファイルを作成
                AssetDatabase.CreateAsset(localUxmlListData, uxmlListPath);
                AssetDatabase.SaveAssets(); // アセットの保存を明示的に行う
                AssetDatabase.ImportAsset(uxmlListPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                Debug.Log($"UIListStore: Successfully created UXMLList asset at: {uxmlListPath}");
            }
            catch (System.Exception e)
            {
                // 詳細なエラーメッセージを出力
                Debug.LogErrorFormat("UIListStore: Error creating UXMLList asset at '{0}': {1}\nStackTrace: {2}", uxmlListPath, e.Message, e.StackTrace);
            }
        }

        // uss管理ファイルがない場合は作る
        string ussListPath = Path.Combine(saveDir, nameof(USSList)) + ".asset";
        USSList localUssListData = AssetDatabase.LoadAssetAtPath<USSList>(ussListPath);

        if (localUssListData == null)
        {
            localUssListData = USSList.CreateInstance<USSList>();

            try
            {
                // ussファイルをローカライズしてコピー
                USSProcessor.DeployAll();
                // 管理ファイルを作成
                AssetDatabase.CreateAsset(localUssListData, ussListPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(ussListPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                Debug.Log($"UIListStore: Successfully created USSList asset at: {ussListPath}");
            }
            catch (System.Exception e)
            {
                // 詳細なエラーメッセージを出力
                Debug.LogErrorFormat("UIListStore: Error creating USSList asset at '{0}': {1}\nStackTrace: {2}", ussListPath, e.Message, e.StackTrace);
            }
        }
       
#endif
        Refresh();
    }

    public void Refresh()
    {
        uxmlListData = Resources.Load<UXMLList>(nameof(UXMLList));
        ussListData = Resources.Load<USSList>(nameof(USSList));

#if UNITY_EDITOR
        if (uxmlListData != null) 
        {
            uxmlListData.LoadUXMLAssets();
        }
       
        if (ussListData != null) 
        {
            ussListData.LoadUSSAssets();
        }
#endif
        if (uxmlListData == null || ussListData == null)
        {
            Debug.LogError("UIListStore: UXMLList or USSList object not found in Resources folder. UI System may not function correctly.");
        }
    }
}
}