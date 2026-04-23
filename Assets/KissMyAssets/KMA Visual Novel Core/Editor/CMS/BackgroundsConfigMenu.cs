using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public class BackgroundsConfigMenu
    {
        [MenuItem(VisualNovelConstants.ToolsPath + "Backgrounds Config", priority = 102)]
        public static void SelectBackgroundConfig()
        {
            BackgroundsInfoConfig config = BackgroundsInfoConfig.Instance;

            if (config == null)
            {
                Debug.LogError($"{typeof(BackgroundsInfoConfig).Name} not found in your project.");
                return;
            }

            Selection.activeObject = config;
            EditorUtility.FocusProjectWindow();
        }
    }
}
