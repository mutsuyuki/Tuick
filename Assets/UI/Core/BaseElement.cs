using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


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

    public BaseElement()
    {
        // インスタンスID決定
        instanceId = ObjectIdFactory.instance.GetNewId();

        // エディタ上だと更新された状態のリストが取れないのでファイルを再度読み込む
#if UNITY_EDITOR
        UIListStore.Instance.Refresh();
#endif

        // テンプレート読み込み
        UXMLList uxmlList = UIListStore.Instance.GetUXMLList();
        templateContainer = uxmlList.GetTemplate(GetType().Name);
        Add(templateContainer);

        // 疑似scopedにするために、全elementにクラスを付与
        templateContainer.AddToClassList(GetType().ToString());
        SetClassNameRecursive(templateContainer);

        // スタイルシート読み込み
        USSList ussList = UIListStore.Instance.GetUSSList();
        StyleSheet styleSheet = ussList.GetTemplate(GetType().Name);
        templateContainer.styleSheets.Add(styleSheet);

        // イベント登録
        RegisterCallback<AttachToPanelEvent>(e => OnAttach(e));
        RegisterCallback<GeometryChangedEvent>(e => OnGeometryChange(e));
    }

    protected virtual void OnAttach(AttachToPanelEvent e)
    {
        if (templateContainer == null)
            return;

        // 簡易Slot
        var slot = SearchSlot(templateContainer);
        if (slot != null)
        {
            ReplaceSlot(slot);
        }
    }

    protected virtual void OnGeometryChange(GeometryChangedEvent e)
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
            var hasTemplate = grandChilds.Where(v => v.GetType() != typeof(TemplateContainer)).Count() > 0;
            var hasSlotTarget = grandChilds.Count() > 1;
            if (isCustomElement && (!hasTemplate || !hasSlotTarget))
                continue;

            child.AddToClassList(GetType().ToString());
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

            if (child.GetType() == typeof(Slot))
            {
                return child as Slot;
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
        var slotIndex = slot.parent.Children().ToList().IndexOf(slot);

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