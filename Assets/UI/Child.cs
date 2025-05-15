using UnityEngine;
using UnityEngine.UIElements;

public class Child : BaseElement
{
    public new class UxmlFactory : UxmlFactory<Child, UxmlTraits>
    {
    }
}