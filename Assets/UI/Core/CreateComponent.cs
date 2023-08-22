#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

public class CreateComponent
{
    [MenuItem("Assets/Create/UI/Component", false, 50)]
    public static void CreateComponentMenu()
    {
        ComponentNameInput.Show(
            new Vector2(100, 100),
            "",
            x => { },
            (x) => CopyTemplates(x),
            "Component name",
            300
        );
    }

    private static async void CopyTemplates(string name)
    {
        if (name == "")
            return;

        string srcPath = Path.Combine(PathUtil.GetLibRootPath(), "Template");
        string distPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(distPath))
        {
            distPath = "Assets";
        }
        else if (Path.GetExtension(distPath) != "")
        {
            distPath = distPath.Replace(Path.GetFileName(distPath), "");
        }

        // name最初の１文字を大文字に
        string nameUpper = name.Substring(0, 1).ToUpper() + name.Substring(1);

        // uxmlファイルはコピーのみ
        string srcUXMLPath = Path.Combine(srcPath, "Template.uxml");
        string distUXMLPath = Path.Combine(distPath, nameUpper + ".uxml");
        File.Copy(srcUXMLPath, distUXMLPath, false);

        // ussファイルは名前を使って内容書き換え
        string srcUSSPath = Path.Combine(srcPath, "Template.uss");
        string distUSSPath = Path.Combine(distPath, nameUpper + ".uss");
        string contentUSS = File.ReadAllText(srcUSSPath);
        contentUSS = contentUSS.Replace("Template", nameUpper);
        File.WriteAllText(distUSSPath, contentUSS);

        // アセットデータベースを更新
        AssetDatabase.Refresh();
        await Task.Delay(1);

        // uxml,ussのリストを更新
        Resources.Load<UXMLList>(nameof(UXMLList)).LoadUXMLAssets();
        Resources.Load<USSList>(nameof(USSList)).LoadUSSAssets();
        AssetDatabase.Refresh();
        await Task.Delay(1);

        // csファイルはクラス名を書き換え
        string srcCSharpPath = Path.Combine(srcPath, "Template.cs");
        string distCSharpPath = Path.Combine(distPath, nameUpper + ".cs");
        string contentCSharp = File.ReadAllText(srcCSharpPath);
        contentCSharp = contentCSharp.Replace("Template", nameUpper);
        File.WriteAllText(distCSharpPath, contentCSharp);
        AssetDatabase.Refresh();
    }
}

#endif