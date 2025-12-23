#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Tuick.Core
{
    public static class TuickMenu
    {
        private const string MENU_PATH = "Assets/Tuick/Force Rebuild All";

        [MenuItem(MENU_PATH, false, 2000)]
        public static void RebuildAll()
        {
            if (!EditorUtility.DisplayDialog("Tuick Rebuild", 
                "This will delete and regenerate the entire 'Tuick/Build' directory.\nThis is useful when the cache is corrupted.\n\nAre you sure?", "Yes", "No"))
            {
                return;
            }

            Debug.Log("[Tuick] Starting Force Rebuild All...");

            // 1. Buildディレクトリのパス取得
            string buildDirPath = PathUtil.GetBuildDirPath();
            
            // 安全のため、もしOS上にディレクトリが存在すれば削除する
            if (Directory.Exists(buildDirPath))
            {
                 try 
                 {
                     // AssetDatabase経由で削除
                     AssetDatabase.DeleteAsset(buildDirPath);
                 } 
                 catch (System.Exception) 
                 {
                     // 強制削除
                     try
                     {
                        Directory.Delete(buildDirPath, true);
                        if (File.Exists(buildDirPath + ".meta")) File.Delete(buildDirPath + ".meta");
                     }
                     catch(System.Exception e)
                     {
                        Debug.LogError($"[Tuick] Failed to delete Build directory: {e.Message}");
                        return; // 削除できなければ中断
                     }
                 }
            }

            // 削除を確定させる
            AssetDatabase.Refresh();

            // 2. 再生成プロセス
            try
            {
                Debug.Log("[Tuick] Regenerating files...");
                
                // UXML/USSのデプロイ
                UXMLProcessor.DeployAll();
                USSProcessor.DeployAll();
                
                // 管理アセット(UXMLList/USSList)の再生成
                UIListStore.Instance.RebuildListAssets();
                
                Debug.Log("[Tuick] Files regenerated.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Tuick] Rebuild failed during regeneration: {e}");
            }

            // 3. 最終更新
            AssetDatabase.Refresh();
            UIListStore.Instance.Refresh();
            
            Debug.Log("[Tuick] Force Rebuild All Complete.");
        }

        // Tuickフォルダが存在する場合のみ有効化（選択位置は問わない）
        [MenuItem(MENU_PATH, true)]
        public static bool RebuildAllValidate()
        {
            // Assets/Tuick フォルダがあるかどうかだけで判断する
            return AssetDatabase.IsValidFolder("Assets/Tuick");
        }
    }
}
#endif
