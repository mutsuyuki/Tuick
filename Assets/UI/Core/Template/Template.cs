using UnityEngine;
using UnityEngine.UIElements;

public sealed class Template : BaseElement
{
    private Label valueLabel;

    public Template()
    {
        valueLabel = contentContainer.Q<Label>("value", instanceId);

        RegisterCallback<PointerDownEvent>(evt =>
        {
            valueLabel.text = (int.Parse(valueLabel.text) + 1).ToString();
        });
    }

    public new class UxmlFactory : UxmlFactory<Template, UxmlTraits>
    {
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        // UXMLの属性を定義
        UxmlIntAttributeDescription _initialValue = new() { name = "int_value" };

        // 初期化処理
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var thisElement = ve as Template;
            var initial_value = _initialValue.GetValueFromBag(bag, cc);
            if (initial_value != 0)
            {
                thisElement.valueLabel.text = initial_value.ToString();
            }
        }
    }
}