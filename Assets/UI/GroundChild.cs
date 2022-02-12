using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class GroundChild : BaseElement
{
    private Label titleLabel;

    public GroundChild() : base("Assets/UI/GroundChild.uxml")
    {
        titleLabel = contentContainer.Q<Label>("title",  instanceId);
    }

    public new class UxmlFactory : UxmlFactory<GroundChild, UxmlTraits>
    {
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        // UXMLの属性を定義
        UxmlIntAttributeDescription _initialValue = new() {name = "int_value"};
        UxmlStringAttributeDescription _initialString = new() {name = "string_value"};

        // 初期化処理
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var groundChild = ve as GroundChild;

            var initial_value = _initialValue.GetValueFromBag(bag, cc);
            var initial_string = _initialString.GetValueFromBag(bag, cc);

            if (initial_value == 0 || initial_string == "")
                return;

            groundChild.titleLabel.text = initial_string + ":" + initial_value;
        }
    }
}