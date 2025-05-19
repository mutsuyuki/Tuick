using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tuick
{
public class BaseElement : VisualElement
{
    protected string instanceId { get; }

    private sealed class ObjectIdFactory
    {
        public static ObjectIdFactory instance { get; } = new();
        private int objectId = 100000;

        public string GetNewId()
        {
            return "el_id_" + objectId++;
        }
    }

    private TemplateContainer templateContainer;
    private bool visualsInitialized;

    public BaseElement()
    {
        // インスタンスID決定
        instanceId = ObjectIdFactory.instance.GetNewId();

        // イベント登録
        RegisterCallback<AttachToPanelEvent>(OnElementAttached);
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    protected virtual void OnElementAttached(AttachToPanelEvent e)
    {
        // 初期化済みかtemplateContainerが何らかの理由で既に存在する場合はスキップ
        if (visualsInitialized || templateContainer != null)
        {
            return;
        }

        // テンプレート読み込み
        UXMLList uxmlList = UIListStore.Instance.GetUXMLList();
        if (uxmlList == null)
        {
            Debug.LogError($"[{GetType().Name} ({instanceId})] UXMLList is null. UIListStore might not be initialized or UXMLList asset is missing. Template cannot be loaded.");
            return;
        }

        string fullTypeName = GetType().FullName;
        templateContainer = uxmlList.GetTemplate(fullTypeName);
        if (templateContainer == null)
        {
            Debug.LogError($"[{GetType().Name} ({instanceId})] Failed to load UXML template for '{fullTypeName}'. Check if UXML file exists and is correctly named in UXMLList.");
            return;
        }
        Add(templateContainer); // テンプレートを自身に追加

        // 疑似scopedにするために、全elementにクラスを付与
        templateContainer.AddToClassList(GetType().Name);
        SetClassNameRecursive(templateContainer);

        // スタイルシート読み込み
        USSList ussList = UIListStore.Instance.GetUSSList();
        if (ussList == null)
        {
            Debug.LogWarning($"[{GetType().Name} ({instanceId})] USSList is null. UIListStore might not be initialized or USSList asset is missing. Stylesheet cannot be loaded.");
        }
        else
        {
            StyleSheet styleSheet = ussList.GetTemplate(fullTypeName);
            if (styleSheet == null)
            {
                Debug.LogWarning($"[{GetType().Name} ({instanceId})] StyleSheet for '{fullTypeName}' not found. Check if USS file exists and is correctly named in USSList.");
            }
            else
            {
                // templateContainer (UXMLのルート) にスタイルシートを追加
                templateContainer.styleSheets.Add(styleSheet);
            }
        }

        // スロット処理
        var slot = SearchSlot(templateContainer);
        if (slot != null)
        {
            ReplaceSlot(slot);
        }

        visualsInitialized = true;
    }

    protected virtual void OnGeometryChanged(GeometryChangedEvent e)
    {
        // Debug.Log(GetType().Name + ":size changed");
    }

    // 自Element内の各要素にクラス名を付与（疑似scoped cssのため）
    private void SetClassNameRecursive(VisualElement element)
    {
        foreach (var child in element.Children())
        {
            // Templateは各カスタムエレメントで設定するのでスキップ
            if (child.GetType() == typeof(TemplateContainer))
                continue;

            // Slot対象のエレメントがある場合、そのエレメントだけは親のクラスにする
            var isCustomElement = child is BaseElement;
            var grandChilds = child.Children().ToList();
            var hasTemplate = grandChilds.Any(v => v.GetType() != typeof(TemplateContainer));
            var hasSlotTarget = grandChilds.Count > 1;
            if (isCustomElement && (!hasTemplate || !hasSlotTarget))
                continue;

            child.AddToClassList(GetType().Name);
            child.AddToClassList(instanceId);
            SetClassNameRecursive(child);
        }
    }

    // Slotがあるかチェック（自Element内のみチェック）
    private Slot SearchSlot(VisualElement element)
    {
        foreach (var child in element.Children())
        {
            var isCustomElement = child is BaseElement;
            if (isCustomElement)
                continue;

            if (child is Slot typedSlot)
            {
                return typedSlot;
            }

            var slot = SearchSlot(child);
            if (slot != null)
            {
                return slot;
            }
        }
        return null;
    }

    // 挟んだElementでSlotを置換
    private void ReplaceSlot(Slot slot)
    {
        // slotがツリーにない場合は何もしない
        if (slot.parent == null) 
            return; 

        var slotIndex = slot.parent.IndexOf(slot);
        var moveTargets
            = Children()
                .Where(v => v.GetType() != typeof(TemplateContainer))
                .ToList();
        moveTargets.Reverse();

        foreach (var target in moveTargets)
        {
            Remove(target);
            slot.parent.Insert(slotIndex, target);
        }

        slot.parent.Remove(slot);
    }
}
}