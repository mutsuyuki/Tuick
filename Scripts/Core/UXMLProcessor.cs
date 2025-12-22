#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace Tuick.Core
{
	public class UXMLProcessor
	{
		// Cache for component names to avoid scanning repeatedly during bulk deploy
		private static HashSet<string> _knownComponentNames;

		public static void DeployAll()
		{
			// Clear cache at start of bulk operation
			_knownComponentNames = null;

			// コピー先を削除
			string uxmlDirPath = PathUtil.GetUXMLDirPath();
			if (Directory.Exists(uxmlDirPath))
			{
				Directory.Delete(uxmlDirPath, true);
			}
			Directory.CreateDirectory(uxmlDirPath);

			// コピー
			List<string> paths = PathUtil.SearchSourceUXMLPaths();
			for (int i = 0; i < paths.Count; i++)
			{
				Deploy(paths[i]);
			}
			
			// Clear cache after operation
			_knownComponentNames = null;
		}

		public static void Deploy(string path)
		{
			if (!File.Exists(path)) return;

			try
			{
				string className = Path.GetFileNameWithoutExtension(path);
				
				// XMLとして読み込み (PreserveWhitespaceで元のフォーマットを極力維持)
				XDocument doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
				
				// 除外判定用のコンポーネント名リストを取得
				var knownComponents = GetKnownComponentNames();

				// 全要素を走査
				foreach (var element in doc.Descendants())
				{
					// 要素名（ローカル名）を取得
					string tagName = element.Name.LocalName;

					// 1. Slotは除外
					if (tagName == "Slot") continue;
					
					// 2. カスタムコンポーネント（他で定義されたTuickコンポーネント）は除外 -> 削除：境界要素としてスタイルを当てやすくするため焼き込む
					// if (knownComponents.Contains(tagName)) continue;

					// クラス属性を取得、なければ作成
					XAttribute classAttr = element.Attribute("class");
					if (classAttr == null)
					{
						element.Add(new XAttribute("class", className));
					}
					else
					{
						// 既にクラスがある場合は追記（重複チェックは簡易的に行う）
						string currentClassValue = classAttr.Value;
						// 単純なContainsだと部分一致してしまう可能性があるため、Splitしてチェック推奨だが
						// パフォーマンス優先で、かつ「焼き込み」は基本的に一度きりの生成物に対して行われるため
						// 確実に追記する。ただし、開発中の頻繁な更新で多重追加されないようにチェック。
						var classes = currentClassValue.Split(' ');
						if (!classes.Contains(className))
						{
							classAttr.Value = currentClassValue + " " + className;
						}
					}
				}

				// 編集後ファイルを保存
				string fileName = Path.GetFileName(path);
				string savePath = Path.Combine(PathUtil.GetUXMLDirPath(), fileName);
				
				// DisableFormattingで、Load時のPreserveWhitespaceを活かして出力（不要な整形を避ける）
				doc.Save(savePath, SaveOptions.DisableFormatting);
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError($"[Tuick] Failed to deploy UXML: {path}\n{e}");
			}
		}

		private static HashSet<string> GetKnownComponentNames()
		{
			if (_knownComponentNames != null) return _knownComponentNames;

			_knownComponentNames = new HashSet<string>();
			var paths = PathUtil.SearchSourceUXMLPaths();
			foreach (var path in paths)
			{
				string name = Path.GetFileNameWithoutExtension(path);
				if (!string.IsNullOrEmpty(name))
				{
					_knownComponentNames.Add(name);
				}
			}
			return _knownComponentNames;
		}
	}
}
#endif