using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : BaseElement
{
    private Label titleLabel;

    public UI() : base("Assets/UI/UI.uxml")
    {
        titleLabel = contentContainer.Q<Label>("title");
        titleLabel.text = Store.Instance.Name;

        Store.Instance
            .ObserveEveryValueChanged(v => v.Name)
            .Subscribe(v => titleLabel.text = v);
    }

    public new class UxmlFactory : UxmlFactory<UI, UxmlTraits>
    {
    }

    protected override void OnPointerDown(PointerDownEvent e)
    {
        Store.Instance.setName("changed_name");
    }
}