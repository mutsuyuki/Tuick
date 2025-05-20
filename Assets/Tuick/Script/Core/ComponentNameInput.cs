#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace Tuick.Core
{
	public class ComponentNameInput : PopupWindowContent
	{
		private const float WINDOW_PADDING = 16.0f;
		private const float BASE_BUTTON_SPACING = 10.0f; // Original spacing, will be multiplied
		private const float CUSTOM_VERTICAL_SPACING_MULTIPLIER = 3f; // Multiplier for vertical spacing
		private const float BUTTON_HORIZONTAL_SPACING_MULTIPLIER = 1f; // Multiplier for horizontal button spacing
		private const float BUTTON_HEIGHT_MULTIPLIER = 2f; // Multiplier for button height

		private string _text;
		private string _message;
		private float _width;
		private Action<string> _changed;
		private Action<string> _closed;
		private GUIStyle _messageLabelStyle;
		private Vector2 _windowSize;
		private bool _didFocus = false;

		private float ActualVerticalSpacing => EditorGUIUtility.standardVerticalSpacing * CUSTOM_VERTICAL_SPACING_MULTIPLIER;
		private float ActualButtonHorizontalSpacing => BASE_BUTTON_SPACING * BUTTON_HORIZONTAL_SPACING_MULTIPLIER;
		private float ActualButtonHeight => EditorGUIUtility.singleLineHeight * BUTTON_HEIGHT_MULTIPLIER;

		public static void Show(
			Vector2 position,
			string text,
			Action<string> changed,
			Action<string> closed,
			string message = null,
			float width = 300
		)
		{
			var rect = new Rect(position, Vector2.zero);
			var content = new ComponentNameInput(text, changed, closed, message, width);
			PopupWindow.Show(rect, content);
		}

		private ComponentNameInput(
			string text,
			Action<string> changed,
			Action<string> closed,
			string message = null,
			float width = 300
		)
		{
			_message = message;
			_text = text;
			_width = width;
			_changed = changed;
			_closed = closed;

			_messageLabelStyle = new GUIStyle(EditorStyles.label);
			_messageLabelStyle.wordWrap = true;
			_messageLabelStyle.fontSize = 14;
			_messageLabelStyle.alignment = TextAnchor.MiddleCenter;

			// ウィンドウサイズを計算する
			var labelWidth = width - (WINDOW_PADDING * 2);
			_windowSize = Vector2.zero;
			_windowSize.x = width;
			_windowSize.y += WINDOW_PADDING; // Top padding
			if (!string.IsNullOrEmpty(message)) // Add height for message only if it exists
			{
				_windowSize.y += _messageLabelStyle.CalcHeight(new GUIContent(message), labelWidth); // Message
				_windowSize.y += ActualVerticalSpacing; // Space after message
			}
			_windowSize.y += EditorGUIUtility.singleLineHeight; // TextField
			_windowSize.y += ActualVerticalSpacing; // Space after TextField
			_windowSize.y += ActualButtonHeight; // Buttons
			_windowSize.y += WINDOW_PADDING; // Bottom padding
		}

		public override void OnGUI(Rect rect)
		{
			if (Event.current.type == EventType.KeyDown)
			{
				// Enterで終了イベント発火して閉じる
				if (
					Event.current.keyCode == KeyCode.Return ||
					Event.current.keyCode == KeyCode.KeypadEnter
				)
				{
					editorWindow.Close();
					_closed?.Invoke(_text);
					Event.current.Use(); // Consume the event to prevent further processing
				}

				// Escで閉じる
				if (Event.current.keyCode == KeyCode.Escape)
				{
					editorWindow.Close();
					Event.current.Use(); // Consume the event
				}
			}

			var textFieldName = $"{GetType().Name}{nameof(_text)}";

			EditorGUILayout.BeginVertical();
			GUILayout.Space(WINDOW_PADDING);

			// タイトルラベル
			if (!string.IsNullOrEmpty(_message))
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace(); // Add flexible space to the left to center the label
				EditorGUILayout.LabelField(_message, _messageLabelStyle, GUILayout.MaxWidth(_width - WINDOW_PADDING * 2));
				GUILayout.FlexibleSpace(); // Add flexible space to the right to center the label
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(ActualVerticalSpacing); // Increased vertical spacing
			}

			// TextFieldを描画
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace(); // Add flexible space to the left to center the text field
			using (var ccs = new EditorGUI.ChangeCheckScope())
			{
				GUI.SetNextControlName(textFieldName);
				_text = EditorGUILayout.TextField(_text, GUILayout.Width(_width - WINDOW_PADDING * 2));
				if (ccs.changed)
				{
					_changed?.Invoke(_text);
				}
			}
			GUILayout.FlexibleSpace(); // Add flexible space to the right to center the text field
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(ActualVerticalSpacing); // Increased vertical spacing

			// ボタンを描画
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace(); // Center the button group

			// Calculate button width for equal size
			float availableWidthForButtons = _width - (WINDOW_PADDING * 2) - ActualButtonHorizontalSpacing;
			float buttonWidth = availableWidthForButtons / 2f;

			if (GUILayout.Button("Cancel", GUILayout.Height(ActualButtonHeight), GUILayout.Width(buttonWidth)))
			{
				editorWindow.Close();
			}
			GUILayout.Space(ActualButtonHorizontalSpacing); // Increased horizontal spacing between buttons

			if (GUILayout.Button("OK", GUILayout.Height(ActualButtonHeight), GUILayout.Width(buttonWidth)))
			{
				editorWindow.Close();
				_closed?.Invoke(_text);
			}

			GUILayout.FlexibleSpace(); // Center the button group
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(WINDOW_PADDING);
			EditorGUILayout.EndVertical();

			// 最初の一回だけ自動的にフォーカスする
			if (!_didFocus)
			{
				GUI.FocusControl(textFieldName);
				_didFocus = true;
			}
		}

		public override Vector2 GetWindowSize() => _windowSize;
	}
}
#endif