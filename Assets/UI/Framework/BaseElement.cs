using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

    public BaseElement(string uxmlPath = "")
    {
        // インスタンスID決定
        instanceId = ObjectIdFactory.instance.GetNewId();

        // テンプレート読み込み
        if (uxmlPath != "")
        {
            var uxmlHandle = Addressables.LoadAssetAsync<VisualTreeAsset>(uxmlPath);
            var visualTreeAsset = uxmlHandle.WaitForCompletion();
            templateContainer = visualTreeAsset.CloneTree();
            Add(templateContainer);

            // 疑似scopedにするために、全elementにクラスを付与
            templateContainer.AddToClassList(GetType().ToString());
            SetClassNameRecursive(templateContainer);
        }

        // イベント登録
        RegisterCallback<AttachToPanelEvent>(e => omitDuplicatedEvent(() => OnAttach(e)));
        RegisterCallback<GeometryChangedEvent>(e => omitDuplicatedEvent(() => OnGeometryChange(e)));

        RegisterCallback<PointerDownEvent>(e => omitDuplicatedEvent(() => OnPointerDown(e)));
        RegisterCallback<PointerMoveEvent>(e => omitDuplicatedEvent(() => OnPointerMove(e)));
        RegisterCallback<PointerUpEvent>(e => omitDuplicatedEvent(() => OnPointerUp(e)));

        RegisterCallback<PointerEnterEvent>(e => omitDuplicatedEvent(() => OnPointerEnter(e)));
        RegisterCallback<PointerOutEvent>(e => omitDuplicatedEvent(() => OnPointerOut(e)));

        RegisterCallback<WheelEvent>(e => omitDuplicatedEvent(() => OnWheel(e)));
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
    }

    protected virtual void OnPointerDown(PointerDownEvent e)
    {
    }

    protected virtual void OnPointerMove(PointerMoveEvent e)
    {
    }

    protected virtual void OnPointerUp(PointerUpEvent e)
    {
    }

    protected virtual void OnPointerEnter(PointerEnterEvent e)
    {
    }

    protected virtual void OnPointerOut(PointerOutEvent e)
    {
    }

    protected virtual void OnWheel(WheelEvent e)
    {
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


    // ------------暫定対応--------------------------------------
    // Linuxだとマウスイベントが同時に２回発生するので、それの暫定回避用
    // Unity側で対応されたら、削除する
    private float previousEventTime = -1;

    private void omitDuplicatedEvent(Action callback)
    {
        var currentTime = Time.time;
        if (currentTime != previousEventTime)
        {
            callback();
        }

        previousEventTime = currentTime;
    }
}