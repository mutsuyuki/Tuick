#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;


public class USSProcessor
{
#if UNITY_EDITOR
    public static void DeployAll()
    {
        // コピー先を削除
        string ussDirPath = PathUtil.GetUSSDirPath();
        Directory.Delete(ussDirPath, true);

        // コピー
        List<string> paths = PathUtil.SearchUSSPaths();
        for (int i = 0; i < paths.Count; i++)
        {
            Deploy(paths[i]);
        }
    }

    public static void Deploy(string path)
    {
        // ussを編集
        string content = File.ReadAllText(path);
        content = Localize(content);

        // 編集後ファイルを保存
        string fileName = Path.GetFileName(path);
        string savePath = Path.Combine(PathUtil.GetUSSDirPath(), fileName);
        File.WriteAllText(savePath, content);
    }

    public static string Localize(string content)
    {
        // ussを編集
        content = "/*-***-*/ \n" + content;

        return content;
    }
#endif
}

#endif