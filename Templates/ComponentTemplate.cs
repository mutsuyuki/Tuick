using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using Tuick;

[UxmlElement]
public sealed partial class ComponentTemplate : BaseElement
{
	[UxmlAttribute("min-value")] private int minValue { get; set; } = 0;
	[UxmlAttribute("max-value")] private int maxValue { get; set; } = 10;
	[UxmlAttribute("init-value")] private int initValue { get; set; } = 0;

	[CreateProperty] private Bindable<string> Message = new("");
	[CreateProperty] private Bindable<int> Count = new(0);

	public ComponentTemplate()
	{
		dataSource = this;
		RegisterCallback<AttachToPanelEvent>(OnElementAttached);
		RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
	}

	private void OnElementAttached(AttachToPanelEvent e)
	{
		Count.Value = initValue;
		UpdateState();

		RegisterCallback<PointerDownEvent>(OnPointerDown);
		this.Q<Button>("minus").clicked += OnMinusClicked;
		this.Q<Button>("plus").clicked += OnPlusClicked;
	}

	private void OnDetachFromPanel(DetachFromPanelEvent e)
	{
		UnregisterCallback<PointerDownEvent>(OnPointerDown);
		this.Q<Button>("minus").clicked -= OnMinusClicked;
		this.Q<Button>("plus").clicked -= OnPlusClicked;
	}
	
	private void UpdateState()
	{
		Count.Value = Mathf.Clamp(Count.Value, minValue, maxValue);

		if (Count.Value >= maxValue)
			Message.Value = "MAX";
		else if (Count.Value <= minValue)
			Message.Value = "MIN";
		else
			Message.Value = "IN RANGE";
	}

	private void OnPointerDown(PointerDownEvent evt)
	{
		Debug.Log("Pointer down event triggered");
		Debug.Log($"Current value: {Count.Value}");
	}

	private void OnPlusClicked()
	{
		Count.Value++;
		UpdateState();
	}

	private void OnMinusClicked()
	{
		Count.Value--;
		UpdateState();
	}
}