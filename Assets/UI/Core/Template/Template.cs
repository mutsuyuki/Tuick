using UnityEngine.UIElements;
using Tuick;

[UxmlElement]
public sealed partial class Template : BaseElement {
	private Label valueLabel;

	[UxmlAttribute("value")] 
	public int IntValue { get; set; } = 0;

	public Template() {
		RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
		RegisterCallback<PointerDownEvent>(OnPointerDown);
	}

	private void OnAttachToPanel(AttachToPanelEvent evt) {
		valueLabel = this.Q<Label>("value", instanceId);
		if (valueLabel != null) {
			valueLabel.text = IntValue.ToString();
		}
	}

	private void OnPointerDown(PointerDownEvent evt) {
		if (valueLabel != null) {
			valueLabel.text = (int.Parse(valueLabel.text) + 1).ToString();
		}
	}
}