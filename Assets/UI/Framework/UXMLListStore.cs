using UnityEngine;

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
        uxmlListData = Resources.Load<UXMLList>("UXMLList");
        if (uxmlListData == null)
        {
            Debug.LogError("UXMLList object not found in Resources folder.");
        }
    }
}