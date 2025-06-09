#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor; // AssetDatabase など

namespace Tuick.Core
{
    public class TuickBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("[TuickBuildPreprocessor] Regenerating Tuick assets for build...");

            // 1. 必要なディレクトリ構造を確保
            string buildDir = PathUtil.GetBuildDirPath();
            if (string.IsNullOrEmpty(buildDir))
            {
                Debug.LogError("[TuickBuildPreprocessor] Failed to get build directory path. Aborting asset regeneration.");
                return;
            }

            // 2. ScriptableObject (UXMLList.asset, USSList.asset) を再生成/更新
            UIListStore.Instance.Refresh();

            // 3. 処理済みUXML/USSファイルなど、その他必要な中間ファイルを再生成/更新
            RegenerateProcessedFiles();

            // 4. 全てのファイル操作が完了したら、Unityにアセットの変更を通知
            AssetDatabase.SaveAssets(); // 未保存の変更をディスクに書き込む
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); // アセットデータベースを強制的に更新

            Debug.Log("[TuickBuildPreprocessor] Tuick assets regeneration complete.");
        }

        private void RegenerateProcessedFiles()
        {
            try
            {
                // UXMLファイルを再生成
                UXMLProcessor.DeployAll();
                
                // USSファイルを再生成
                USSProcessor.DeployAll();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TuickBuildPreprocessor] Error regenerating processed files: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
#endif