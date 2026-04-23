using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public class ActorsConfigMenu
    {
        [MenuItem(VisualNovelConstants.ToolsPath + "Actors Config", priority = 101)]
        public static void SelectActorsConfig()
        {
            ActorsInfoConfig config = ActorsInfoConfig.Instance;

            if (config == null)
            {
                Debug.LogError($"{typeof(ActorsInfoConfig).Name} not found in your project.");
                return;
            }

            Selection.activeObject = config;
            EditorUtility.FocusProjectWindow();
        }
    }
}
