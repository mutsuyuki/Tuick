#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

namespace Tuick
{
	public class UXMLProcessor
	{
		public static void DeployAll()
		{
			// コピー先を削除
			string uxmlDirPath = PathUtil.GetUXMLDirPath();
			Directory.Delete(uxmlDirPath, true);

			// コピー
			List<string> paths = PathUtil.SearchSourceUXMLPaths();
			for (int i = 0; i < paths.Count; i++)
			{
				Deploy(paths[i]);
			}
		}

		public static void Deploy(string path)
		{
			// uxmlを編集（いまのところuxmlは編集なし）
			string content = File.ReadAllText(path);

			// 編集後ファイルを保存
			string fileName = Path.GetFileName(path);
			string savePath = Path.Combine(PathUtil.GetUXMLDirPath(), fileName);
			File.WriteAllText(savePath, content);
		}
	}
}
#endif