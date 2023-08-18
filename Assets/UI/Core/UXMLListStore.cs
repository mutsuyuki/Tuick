using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEditor;
#endif

public class UXMLListStore
{
    // プライベートコンストラクタ
    private UXMLListStore()
    {
    }

    // uxmlリストデータ
    private UXMLList uxmlListData;

    public UXMLList GetUXMLList()
    {
        return uxmlListData;
    }


    // シングルトン化
    private static UXMLListStore _instance;

    public static UXMLListStore Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UXMLListStore();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    private void Initialize()
    {
#if UNITY_EDITOR
        // 本スクリプトの場所を取得
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        string[] assetGuids = AssetDatabase.FindAssets(className + " t:script");
        if (assetGuids.Length > 1)
        {
            Debug.LogError("Multiple " + className + " found.");
            return;
        }

        // 保存先のディレクトリパスを取得。（ディレクトリがなければ作る）
        string scriptPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        string saveDir = Path.Combine(scriptDirectory, "Temp", "Resources");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        // UXMLListオブジェクトを作成
        string savePath = Path.Combine(saveDir, nameof(UXMLList)) + ".asset";
        if (!AssetDatabase.LoadAssetAtPath<UXMLList>(savePath))
        {
            UXMLList uxmlListData = UXMLList.CreateInstance<UXMLList>();

            try
            {
                AssetDatabase.CreateAsset(uxmlListData, savePath);
            }
            catch (System.Exception e)
            {
                // アセット作成直後のインポートでエラーが起きるが、実際には作成されててロードもできるので、とりあえず無視する。
                Debug.Log("please ignore this error if import uxml file correctly.");
            }
            AssetDatabase.Refresh();
        }

        // UXMLListオブジェクトにuxmlをロード
        Resources.Load<UXMLList>(nameof(UXMLList)).LoadUXMLAssets();

#endif
        uxmlListData = Resources.Load<UXMLList>(nameof(UXMLList));
        if (uxmlListData == null)
        {
            Debug.LogError("UXMLList object not found in Resources folder.");
        }
    }
}