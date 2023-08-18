using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
#endif

public class UIListStore
{
    // プライベートコンストラクタ
    private UIListStore()
    {
    }

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
        // Resourcesフォルダのパスを取得
        string saveDir = PathUtil.GetResourcesDirPath();


        // uxmlファイルをコピー
        UXMLProcessor.DeployAll();

        // umxl管理ファイルがない場合は作る
        string uxmlListPath = Path.Combine(saveDir, nameof(UXMLList)) + ".asset";
        if (!AssetDatabase.LoadAssetAtPath<UXMLList>(uxmlListPath))
        {
            UXMLList uxmlListData = UXMLList.CreateInstance<UXMLList>();

            try
            {
                AssetDatabase.CreateAsset(uxmlListData, uxmlListPath);
            }
            catch (System.Exception e)
            {
                // アセット作成直後のインポートでエラーが起きるが、実際には作成されててロードもできるので、とりあえず無視する。
                Debug.Log("please ignore this error if import uxml file correctly.");
                Debug.Log(e.Message);
            }
        }


        // ussファイルをローカライズしてコピー
        USSProcessor.DeployAll();

        // uss管理ファイルがない場合は作る
        string ussListPath = Path.Combine(saveDir, nameof(USSList)) + ".asset";
        if (!AssetDatabase.LoadAssetAtPath<USSList>(ussListPath))
        {
            USSList ussListData = USSList.CreateInstance<USSList>();

            try
            {
                AssetDatabase.CreateAsset(ussListData, ussListPath);
            }
            catch (System.Exception e)
            {
                // アセット作成直後のインポートでエラーが起きるが、実際には作成されててロードもできるので、とりあえず無視する。
                Debug.Log("please ignore this error if import uss file correctly.");
                Debug.Log(e.Message);
            }
        }

        // 管理ファイルを更新
        AssetDatabase.Refresh();

        uxmlListData = Resources.Load<UXMLList>(nameof(UXMLList));
        uxmlListData.LoadUXMLAssets();
        ussListData = Resources.Load<USSList>(nameof(USSList));
        ussListData.LoadUSSAssets();
#endif
        uxmlListData = Resources.Load<UXMLList>(nameof(UXMLList));
        ussListData = Resources.Load<USSList>(nameof(USSList));
        if (uxmlListData == null || ussListData == null)
        {
            Debug.LogError("UXMLList or USSList object not found in Resources folder.");
        }
    }
}