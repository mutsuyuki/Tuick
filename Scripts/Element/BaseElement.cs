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

			// 0. 既存の子要素（スロットに挿入されるべきコンテンツ）を退避
			// TemplateContainerを展開すると混ざってしまうため、先に取り出しておく
			var slotContents = Children().ToList();
			Clear();

			if (hasUxml)
			{
				// 1. TemplateContainerの中身を自分自身に移植
				// これにより、余計なTemplateContainer階層を排除し、:rootがこのElement自身を指すようにする。
				while (templateContainer.childCount > 0)
				{
					var child = templateContainer.ElementAt(0);
					Add(child);
				}
				// 中身は空になったが、参照は保持しておく（GC任せ）
			}
			else
			{
				// UXMLがない場合は、退避したコンテンツをそのまま戻す（単なるコンテナとして振る舞う）
				foreach (var content in slotContents)
				{
					Add(content);
				}
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
			AddToClassList(GetType().Name);

			// スタイルシート適用
			// 常に自分自身(this)に適用する。これで :root セレクタはこのElement自身を指すことになる。
			if (hasUss)
			{
				styleSheets.Add(styleSheet);
			}

			// スロット処理 (UXMLがある場合のみ)
			if (hasUxml && slotContents.Count > 0)
			{
				// 自分自身(this)からスロットを探す
				var slot = SearchSlot(this);
				if (slot != null)
				{
					InsertSlotContents(slot, slotContents);
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

		// スロットの位置にコンテンツを挿入する
		private void InsertSlotContents(Slot slot, System.Collections.Generic.List<VisualElement> contents)
		{
			// slotがツリーにない場合は何もしない
			if (slot.parent == null)
				return;

			slot.parent.AddToClassList(SlotHostClass);

			var slotIndex = slot.parent.IndexOf(slot);
			
			// 挿入順序を維持するため、逆順にして同じインデックスに挿入する
			// 例: contents=[A, B], index=0
			// 1. Insert B at 0 -> [B, slot]
			// 2. Insert A at 0 -> [A, B, slot]
			var reverseContents = new System.Collections.Generic.List<VisualElement>(contents);
			reverseContents.Reverse();

			foreach (var target in reverseContents)
			{
				target.AddToClassList(SlotItemClass);
				// targetは既にClear()で親から外れているのでRemove不要
				slot.parent.Insert(slotIndex, target);
			}

			slot.parent.Remove(slot);
		}
	}
}