#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace Tuick
{
	public class ComponentNameInput : PopupWindowContent
	{
		private const float WINDOW_PADDING = 16.0f;

		private string _text;
		private string _message;
		private float _width;
		private Action<string> _changed;
		private Action<string> _closed;
		private GUIStyle _messageLabelStyle;
		private Vector2 _windowSize;
		private bool _didFocus = false;

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
			_windowSize.y += WINDOW_PADDING; // Space
			_windowSize.y += WINDOW_PADDING; // Space
			_windowSize.y += _messageLabelStyle.CalcHeight(new GUIContent(message), labelWidth); // Message
			_windowSize.y += WINDOW_PADDING; // Space
			_windowSize.y += EditorGUIUtility.standardVerticalSpacing; // Space
			_windowSize.y += EditorGUIUtility.singleLineHeight; // TextField
			_windowSize.y += WINDOW_PADDING; // Space
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
				}

				// Escで閉じる
				if (Event.current.keyCode == KeyCode.Escape)
				{
					editorWindow.Close();
				}
			}

			var textFieldName = $"{GetType().Name}{nameof(_text)}";
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(WINDOW_PADDING);
				using (new EditorGUILayout.VerticalScope())
				{
					// タイトルラベル
					GUILayout.Space(WINDOW_PADDING);
					EditorGUILayout.LabelField(_message, _messageLabelStyle);
					GUILayout.Space(WINDOW_PADDING);

					// TextFieldを描画
					using (var ccs = new EditorGUI.ChangeCheckScope())
					{
						GUI.SetNextControlName(textFieldName);
						_text = EditorGUILayout.TextField(_text);
						if (ccs.changed)
						{
							_changed?.Invoke(_text);
						}
					}
				}

				GUILayout.Space(WINDOW_PADDING);
			}

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