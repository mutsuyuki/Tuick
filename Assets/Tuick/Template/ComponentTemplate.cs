using UnityEngine.UIElements;
using Tuick;

[UxmlElement]
public sealed partial class ComponentTemplate : BaseElement
{
	private Label valueLabel;

	[UxmlAttribute("value")] public int value { get; set; } = 0;

	public ComponentTemplate()
	{
		RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
		RegisterCallback<PointerDownEvent>(OnPointerDown);
	}

	private void OnAttachToPanel(AttachToPanelEvent evt)
	{
		valueLabel = this.Q<Label>("value", instanceId);
		if (valueLabel != null)
		{
			valueLabel.text = value.ToString();
		}
	}

	private void OnPointerDown(PointerDownEvent evt)
	{
		if (valueLabel != null)
		{
			valueLabel.text = (int.Parse(valueLabel.text) + 1).ToString();
		}
	}
}