using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : BaseElement
{
    private Label titleLabel;

    public UI()
    {
        titleLabel = contentContainer.Q<Label>("title");
        titleLabel.text = Store.Instance.Name;

        Store.Instance
            .ObserveEveryValueChanged(v => v.Name)
            .Subscribe(v => titleLabel.text = v);

        titleLabel.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    public new class UxmlFactory : UxmlFactory<UI, UxmlTraits>
    {
    }

    private void OnPointerDown(PointerDownEvent e)
    {
        Store.Instance.setName("changed_name");
    }
}