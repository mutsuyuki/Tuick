#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


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
        var regex = new Regex(@"([a-z0-9_-]+)", RegexOptions.IgnoreCase);
        List<string> tokens = SplitString(property);
        string modifiedSelector = "";
        foreach (var token in tokens)
        {
            modifiedSelector += token;
            if (regex.IsMatch(token))
            {
                modifiedSelector += "." + className;
            }
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

#endif