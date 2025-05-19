using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
#endif

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

    private async void Initialize()
    {
#if UNITY_EDITOR
        // Resourcesフォルダのパスを取得
        string saveDir = PathUtil.GetResourcesDirPath();

        // umxl管理ファイルがない場合は作る
        string uxmlListPath = Path.Combine(saveDir, nameof(UXMLList)) + ".asset";
        UXMLList localUxmlListData = AssetDatabase.LoadAssetAtPath<UXMLList>(uxmlListPath);
        bool uxmlCreationSuccess = true;

        if (localUxmlListData == null)
        {
            localUxmlListData = UXMLList.CreateInstance<UXMLList>();
            uxmlCreationSuccess = false;

            try
            {
                // uxmlファイルをコピー
                UXMLProcessor.DeployAll();
                // 管理ファイルを作成
                AssetDatabase.CreateAsset(localUxmlListData, uxmlListPath);
                AssetDatabase.SaveAssets(); // アセットの保存を明示的に行う
                Debug.Log($"Successfully created UXMLList asset at: {uxmlListPath}");
                uxmlCreationSuccess = true;
            }
            catch (System.Exception e)
            {
                // 詳細なエラーメッセージを出力
                Debug.LogErrorFormat("Error creating UXMLList asset at '{0}': {1}\nStackTrace: {2}", uxmlListPath, e.Message, e.StackTrace);
                // エラーが発生した場合は変数をnullに設定
                localUxmlListData = null;
            }
        }

        // uss管理ファイルがない場合は作る
        string ussListPath = Path.Combine(saveDir, nameof(USSList)) + ".asset";
        USSList localUssListData = AssetDatabase.LoadAssetAtPath<USSList>(ussListPath);
        bool ussCreationSuccess = true;

        if (localUssListData == null)
        {
            localUssListData = USSList.CreateInstance<USSList>();
            ussCreationSuccess = false;

            try
            {
                // ussファイルをローカライズしてコピー
                USSProcessor.DeployAll();
                // 管理ファイルを作成
                AssetDatabase.CreateAsset(localUssListData, ussListPath);
                AssetDatabase.SaveAssets(); // アセットの保存を明示的に行う
                Debug.Log($"Successfully created USSList asset at: {ussListPath}");
                ussCreationSuccess = true;
            }
            catch (System.Exception e)
            {
                // 詳細なエラーメッセージを出力
                Debug.LogErrorFormat("Error creating USSList asset at '{0}': {1}\nStackTrace: {2}", ussListPath, e.Message, e.StackTrace);
                // エラーが発生した場合は変数をnullに設定
                localUssListData = null;
            }
        }

        // 管理ファイルを更新
        AssetDatabase.Refresh();
        await Task.Delay(1000);
        Refresh();
        await Task.Delay(1000);

        // アセットのロードが成功したか確認してから処理を続行
        if (_instance.uxmlListData != null)
        {
            _instance.uxmlListData.LoadUXMLAssets();
        }
        else if (!uxmlCreationSuccess)
        {
            Debug.LogWarning($"UXMLList asset could not be loaded or created at: {uxmlListPath}");
        }

        if (_instance.ussListData != null)
        {
            _instance.ussListData.LoadUSSAssets();
        }
        else if (!ussCreationSuccess)
        {
            Debug.LogWarning($"USSList asset could not be loaded or created at: {ussListPath}");
        }
#endif
        Refresh();
    }

    public void Refresh()
    {
        uxmlListData = Resources.Load<UXMLList>(nameof(UXMLList));
        ussListData = Resources.Load<USSList>(nameof(USSList));
        if (uxmlListData == null || ussListData == null)
        {
            Debug.LogError("UXMLList or USSList object not found in Resources folder.");
        }
    }
}