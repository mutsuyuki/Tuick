#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tuick
{
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

    private static void CopyTemplates(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        string srcPath = PathUtil.GetLibRootPath();
        if (string.IsNullOrEmpty(srcPath))
        {
            Debug.LogError("CreateComponent: Could not determine library root path. Component creation aborted.");
            return;
        }
        srcPath = Path.Combine(srcPath, "Template");

        string distPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(distPath))
        {
            distPath = "Assets";
        }
        else if (Path.GetExtension(distPath) != "")
        {
            distPath = Path.GetDirectoryName(distPath);
        }

        // name最初の１文字を大文字に
        string nameUpper = char.ToUpper(name[0]) + name.Substring(1);

        // ビルドプロセスを開始
        AssetDatabase.StartAssetEditing();
        try
        {
            // uxmlファイルはコピーのみ
            string srcUXMLPath = Path.Combine(srcPath, "Template.uxml");
            string distUXMLPath = Path.Combine(distPath, nameUpper + ".uxml");
            if (File.Exists(srcUXMLPath))
            {
                File.Copy(srcUXMLPath, distUXMLPath, false);
                AssetDatabase.ImportAsset(distUXMLPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
            else
            {
                Debug.LogError($"CreateComponent: Template UXML file not found at {srcUXMLPath}");
            }

            // ussファイルは名前を使って内容書き換え
            string srcUSSPath = Path.Combine(srcPath, "Template.uss");
            string distUSSPath = Path.Combine(distPath, nameUpper + ".uss");
            if (File.Exists(srcUSSPath))
            {
                string contentUSS = File.ReadAllText(srcUSSPath);
                contentUSS = contentUSS.Replace("Template", nameUpper);
                File.WriteAllText(distUSSPath, contentUSS);
                AssetDatabase.ImportAsset(distUSSPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
            else
            {
                Debug.LogError($"CreateComponent: Template USS file not found at {srcUSSPath}");
            }

            // csファイルはクラス名を書き換え
            string srcCSharpPath = Path.Combine(srcPath, "Template.cs");
            string distCSharpPath = Path.Combine(distPath, nameUpper + ".cs");
            if (File.Exists(srcCSharpPath))
            {
                string contentCSharp = File.ReadAllText(srcCSharpPath);
                contentCSharp = contentCSharp.Replace("Template", nameUpper);
                File.WriteAllText(distCSharpPath, contentCSharp);
                AssetDatabase.ImportAsset(distCSharpPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
            else
            {
                Debug.LogError($"CreateComponent: Template C# file not found at {srcCSharpPath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CreateComponent: Error during template copying for '{nameUpper}'. Exception: {e}");
        }
        finally
        {
            AssetDatabase.StopAssetEditing(); // アセット操作を終了し、インポートをトリガー
        }

        // uxml,ussのリストを更新 (次のエディタフレームで実行)
        EditorApplication.delayCall += () =>
        {
            UIListStore.Instance.Refresh();
            Debug.Log($"CreateComponent: Updating UXML/USS lists after creating '{nameUpper}'.");
        };
    }
}
}

#endif
