using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : BaseElement
{
    private Label titleLabel;

    public UI()
    {
        titleLabel = contentContainer.Q<Label>("title");
        if (titleLabel == null)
        {
            Debug.Log("titleLabel is null");
            return;
        }
        titleLabel.text = "first state";

        titleLabel.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    public new class UxmlFactory : UxmlFactory<UI, UxmlTraits>
    {
    }

    private void OnPointerDown(PointerDownEvent e)
    {
        titleLabel.text = "second state";
    }
}