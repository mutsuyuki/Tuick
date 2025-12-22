#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Tuick.Core
{
	public class USSProcessor
	{
		public static void DeployAll()
		{
			// コピー先を削除
			string ussDirPath = PathUtil.GetUSSDirPath();
			Directory.Delete(ussDirPath, true);

			// コピー
			List<string> paths = PathUtil.SearchSourceUSSPaths();
			for (int i = 0; i < paths.Count; i++)
			{
				Deploy(paths[i]);
			}
		}

		public static void Deploy(string path)
		{
			// ussを編集
			string content = File.ReadAllText(path);
			string className = Path.GetFileNameWithoutExtension(path);
			content = Localize(content, className);

			// 編集後ファイルを保存
			string fileName = Path.GetFileName(path);
			string savePath = Path.Combine(PathUtil.GetUSSDirPath(), fileName);
			File.WriteAllText(savePath, content);
		}

		private static string Localize(string ussBody, string className)
		{
			List<string> properties;
			List<string> settings;
			ParseUSS(ussBody, out properties, out settings);

			for (int i = 0; i < properties.Count(); i++)
			{
				properties[i] = addClass(properties[i], className);
			}

			string result = "";
			int max = Mathf.Min(properties.Count, settings.Count);
			for (int i = 0; i < max; i++)
			{
				result += properties[i] + " {\n    " + settings[i] + "\n}\n\n";
			}

			return result;
		}

		public static void ParseUSS(string css, out List<string> properties, out List<string> settings)
		{
			properties = new List<string>();
			settings = new List<string>();

			var matches = Regex.Matches(css, @"([^\{\}]+)\{([^\{\}]*)\}");

			foreach (Match match in matches)
			{
				properties.Add(match.Groups[1].Value.Trim());
				settings.Add(match.Groups[2].Value.Trim());
			}
		}


		// クラスを付与する
		public static string addClass(string property, string className)
		{
			var word = new Regex(@"^[a-z0-9_-]+$", RegexOptions.IgnoreCase);
			List<string> tokens = SplitString(property);
			string modifiedSelector = "";

			// よく使う疑似クラスのみ最小対応（関数系疑似は触らない）
			var pseudos = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
			{
				"hover", "active", "focus", "disabled", "enabled", "checked", "selected"
			};

			// attribute selector 内は触らない（破壊回避）
			bool inAttr = false;

			for (int i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];

				// attribute selector の開始/終了判定
				if (token.Contains("[")) inAttr = true;

				// 疑似クラス名に付く .className を「: の直前」に移す（意味は変えない）
				// 例) ".x.AABB:hover.AABB" → ".x.AABB.AABB:hover"
				if (!inAttr && token.Contains(":") && i + 1 < tokens.Count)
				{
					var next = tokens[i + 1];
					if (word.IsMatch(next) && pseudos.Contains(next))
					{
						modifiedSelector += "." + className;
					}
				}

				modifiedSelector += token;

				if (word.IsMatch(token))
				{
					// attribute selector 内では付与しない（破壊回避）
					if (inAttr) { /* no-op */ }
					else
					{
						// 直前が ":" かつ対象疑似クラス名なら、後ろには付与しない（上で移動済み）
						if (i > 0 && tokens[i - 1].Contains(":") && pseudos.Contains(token))
						{
							// no-op
						}
						else
						{
							modifiedSelector += "." + className;
						}
					}
				}

				if (token.Contains("]")) inAttr = false;
			}

			return modifiedSelector;
		}

		// クラス名やタグ名とそれ以外に分離した配列に
		public static List<string> SplitString(string input)
		{
			var regex = new Regex(@"([a-z0-9_-]+|[^a-z0-9_-]+)", RegexOptions.IgnoreCase);
			var matches = regex.Matches(input);

			var results = new List<string>();
			foreach (Match match in matches)
			{
				results.Add(match.Value);
			}

			return results;
		}
	}
}

#endif