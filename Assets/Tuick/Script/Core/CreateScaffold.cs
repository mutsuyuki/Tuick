#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tuick.Core
{
    public class CreateScaffold
    {
        private const string UXML_DOCUMENT_TEMPLATE_FILENAME = "UIDocumentTemplate.uxml";
        private const string COMPONENT_CS_TEMPLATE_FILENAME = "ComponentTemplate.cs";
        private const string COMPONENT_UXML_TEMPLATE_FILENAME = "ComponentTemplate.uxml";
        private const string COMPONENT_USS_TEMPLATE_FILENAME = "ComponentTemplate.uss";
        private const string PLACEHOLDER_TAG = "<ComponentTemplate />";

        [MenuItem("Assets/Create/Tuick/Scaffold", false, 40)] // Componentより若い番号で先に表示
        public static void CreateScaffoldMenu()
        {
            ComponentNameInput.Show(
                new Vector2(100, 100), // position
                "",                   // default text
                changed: x => { },      // on changed action
                closed: (name, createFolder) => GenerateScaffold(name, createFolder), // on closed action
                message: "Enter the name for the new UI Scaffold\n(e.g., MapUI, StatusUI)", // input window message
                width: 350             // window width
            );
        }

        private static void GenerateScaffold(string name, bool createFolder)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Scaffold name cannot be empty.");
                return;
            }

            string libTemplatePath = PathUtil.GetLibRootPath();
            if (string.IsNullOrEmpty(libTemplatePath))
            {
                Debug.LogError("CreateScaffold: Could not determine library root path. Scaffold creation aborted.");
                return;
            }
            libTemplatePath = Path.Combine(libTemplatePath, "Template");

            string currentAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(currentAssetPath))
            {
                currentAssetPath = "Assets";
            }
            else if (Path.GetExtension(currentAssetPath) != "") // If a file is selected, get its directory
            {
                currentAssetPath = Path.GetDirectoryName(currentAssetPath);
            }

            string nameUpper = char.ToUpper(name[0]) + name.Substring(1);
            string documentAssetName = nameUpper + "UIDocument"; // e.g., MapUIDocument

            string finalDistPath = currentAssetPath;
            if (createFolder)
            {
                finalDistPath = Path.Combine(currentAssetPath, nameUpper);
                if (!Directory.Exists(finalDistPath))
                {
                    Directory.CreateDirectory(finalDistPath);
                    // Refresh is needed to make sure Unity sees the new folder before creating assets in it
                    AssetDatabase.Refresh();
                }
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                // 1. Create [Name]UIDocument.uxml
                string srcDocUXMLPath = Path.Combine(libTemplatePath, UXML_DOCUMENT_TEMPLATE_FILENAME);
                string distDocUXMLPath = Path.Combine(finalDistPath, documentAssetName + ".uxml");

                if (File.Exists(srcDocUXMLPath))
                {
                    string docContent = File.ReadAllText(srcDocUXMLPath);
                    // Replace placeholder with actual component tag (e.g., <MapUI />)
                    // Assuming the component will be in the global namespace or properly imported
                    string componentTag = $"<{nameUpper} />"; 
                    docContent = docContent.Replace(PLACEHOLDER_TAG, componentTag);
                    File.WriteAllText(distDocUXMLPath, docContent);
                    AssetDatabase.ImportAsset(distDocUXMLPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                    Debug.Log($"Generated {distDocUXMLPath}");
                }
                else
                {
                    Debug.LogError($"CreateScaffold: UIDocument Template UXML file not found at {srcDocUXMLPath}");
                    return; // Stop if main document template is missing
                }

                // 2. Create [Name].uxml (from ComponentTemplate.uxml)
                string srcCompUXMLPath = Path.Combine(libTemplatePath, COMPONENT_UXML_TEMPLATE_FILENAME);
                string distCompUXMLPath = Path.Combine(finalDistPath, nameUpper + ".uxml");
                if (File.Exists(srcCompUXMLPath))
                {
                    File.Copy(srcCompUXMLPath, distCompUXMLPath, false); // No content replacement needed for uxml itself
                    AssetDatabase.ImportAsset(distCompUXMLPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                }
                else
                {
                    Debug.LogError($"CreateScaffold: Component Template UXML file not found at {srcCompUXMLPath}");
                }

                // 3. Create [Name].uss (from ComponentTemplate.uss)
                string srcCompUSSPath = Path.Combine(libTemplatePath, COMPONENT_USS_TEMPLATE_FILENAME);
                string distCompUSSPath = Path.Combine(finalDistPath, nameUpper + ".uss");
                if (File.Exists(srcCompUSSPath))
                {
                    string contentUSS = File.ReadAllText(srcCompUSSPath);
                    contentUSS = contentUSS.Replace("ComponentTemplate", nameUpper); // Replace class name in USS
                    File.WriteAllText(distCompUSSPath, contentUSS);
                    AssetDatabase.ImportAsset(distCompUSSPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                }
                else
                {
                    Debug.LogError($"CreateScaffold: Component Template USS file not found at {srcCompUSSPath}");
                }

                // 4. Create [Name].cs (from ComponentTemplate.cs)
                string srcCompCSPath = Path.Combine(libTemplatePath, COMPONENT_CS_TEMPLATE_FILENAME);
                string distCompCSPath = Path.Combine(finalDistPath, nameUpper + ".cs");
                if (File.Exists(srcCompCSPath))
                {
                    string contentCS = File.ReadAllText(srcCompCSPath);
                    contentCS = contentCS.Replace("ComponentTemplate", nameUpper); // Replace class name in C#
                    File.WriteAllText(distCompCSPath, contentCS);
                    AssetDatabase.ImportAsset(distCompCSPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                }
                else
                {
                    Debug.LogError($"CreateScaffold: Component Template C# file not found at {srcCompCSPath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CreateScaffold: Error during scaffold generation for '{nameUpper}'. Exception: {e}");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh(); // Ensure all created assets are recognized
            }

            // Update UXML/USS lists after creating scaffold
            EditorApplication.delayCall += () => 
            {
                UIListStore.Instance.Refresh();
                Debug.Log($"CreateScaffold: Updating UXML/USS lists after creating scaffold '{nameUpper}'.");
            };
        }
    }
}
#endif
