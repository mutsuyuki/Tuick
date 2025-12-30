using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Tuick.Core;

namespace Tuick
{
	
	[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
	public class FullScreenAttribute : System.Attribute { }

	public class BaseElement : VisualElement
	{
		protected string instanceId { get; }

		private sealed class ObjectIdFactory
		{
			public static ObjectIdFactory instance { get; } = new();
			private int objectId = 100000;

			public string GetNewId()
			{
				return "el_id_" + objectId++;
			}
		}

		private TemplateContainer templateContainer;
		private bool visualsInitialized;

		public BaseElement()
		{
			// インスタンスID決定
			instanceId = ObjectIdFactory.instance.GetNewId();

			// イベント登録
			RegisterCallback<AttachToPanelEvent>(OnElementAttached);
		}

		private void OnElementAttached(AttachToPanelEvent e)
		{
			// 初期化済みかtemplateContainerが何らかの理由で既に存在する場合はスキップ
			if (visualsInitialized || templateContainer != null)
			{
				return;
			}

			// リスト取得
			UXMLList uxmlList = UIListStore.Instance.GetUXMLList();
			USSList ussList = UIListStore.Instance.GetUSSList();

			if (uxmlList == null || ussList == null)
			{
				Debug.LogError(
					$"[{GetType().Name} ({instanceId})] UIListStore not initialized properly. UXMLList or USSList is null."
				);
				return;
			}

			string fullTypeName = GetType().FullName;

			// テンプレートとスタイルの取得
			templateContainer = uxmlList.GetTemplate(fullTypeName);
			StyleSheet styleSheet = ussList.GetTemplate(fullTypeName);

			bool hasUxml = templateContainer != null;
			bool hasUss = styleSheet != null;

			// 不足情報のログ集約 (Info)
			if (!hasUxml || !hasUss)
			{
				string missing = (!hasUxml && !hasUss) ? "UXML & USS" : (!hasUxml ? "UXML" : "USS");
				Debug.Log($"[{GetType().Name} ({instanceId})] Initialized without {missing}. (Flexible mode)");
			}

			VisualElement styleTarget = this;

			if (hasUxml)
			{
				Add(templateContainer);
				styleTarget = templateContainer;

				// コンテナのサイズを親に合わせるようにしておく
				templateContainer.style.flexGrow = 1;
				templateContainer.style.width = Length.Percent(100);
				templateContainer.style.height = Length.Percent(100);
			}

			// [FullScreen] 属性対応
			if (System.Attribute.IsDefined(GetType(), typeof(FullScreenAttribute)))
			{
				style.position = Position.Absolute;
				style.left = 0;
				style.top = 0;
				style.right = 0;
				style.bottom = 0;
				style.width = Length.Percent(100);
				style.height = Length.Percent(100);
			}

			// 疑似scopedにするためにクラスを付与
			styleTarget.AddToClassList(GetType().Name);

			// スタイルシート適用
			if (hasUss)
			{
				styleTarget.styleSheets.Add(styleSheet);
			}

			// スロット処理 (UXMLがある場合のみ)
			if (hasUxml)
			{
				var slot = SearchSlot(templateContainer);
				if (slot != null)
				{
					ReplaceSlot(slot);
				}
			}

			visualsInitialized = true;
		}

		// Slotがあるかチェック（自Element内のみチェック）
		private Slot SearchSlot(VisualElement element)
		{
			foreach (var child in element.Children())
			{
				var isCustomElement = child is BaseElement;
				if (isCustomElement)
					continue;

				if (child is Slot typedSlot)
				{
					return typedSlot;
				}

				var slot = SearchSlot(child);
				if (slot != null)
				{
					return slot;
				}
			}

			return null;
		}

		private const string SlotHostClass = "tuick-slot-host";
		private const string SlotItemClass = "tuick-slot-item";

		// 挟んだElementでSlotを置換
		private void ReplaceSlot(Slot slot)
		{
			// slotがツリーにない場合は何もしない
			if (slot.parent == null)
				return;

			slot.parent.AddToClassList(SlotHostClass);

			var slotIndex = slot.parent.IndexOf(slot);
			var moveTargets
				= Children()
					.Where(v => v.GetType() != typeof(TemplateContainer))
					.ToList();
			moveTargets.Reverse();

			foreach (var target in moveTargets)
			{
				target.AddToClassList(SlotItemClass);
				Remove(target);
				slot.parent.Insert(slotIndex, target);
			}

			slot.parent.Remove(slot);
		}
	}
}