using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Utility class to add a menu item for opening the Dialogue Creator window in the Unity Editor's 'Tools' menu.
    /// </summary>
    public static class DialogueBuilderMenu
    {
        private const string MenuItemPath = VisualNovelConstants.ToolsPath + "Dialogue Creator";

        /// <summary>
        /// Opens the <see cref="DialogueCreatorWindow"/> editor window.
        /// </summary>
        [MenuItem(MenuItemPath)]
        public static void ShowWindow()
        {
            // Create or get the existing window instance
            var window = EditorWindow.GetWindow<DialogueCreatorWindow>(false, "Dialogue Builder", true);

            // Ensure the window has a reasonable minimum size
            window.minSize = new Vector2(360, 360);

            CenterWindow(window);
        }

        /// <summary>
        /// Calculates and sets the window's position to center it on the main screen, 
        /// setting a default size of half the screen resolution.
        /// </summary>
        /// <param name="window">The editor window instance to center.</param>
        private static void CenterWindow(EditorWindow window)
        {
            var res = Screen.currentResolution;
            float ppp = EditorGUIUtility.pixelsPerPoint; // DPI scale (2 on Retina, etc.)

            // Fallback screen dimensions
            int screenW = res.width > 0 ? res.width : 1920;
            int screenH = res.height > 0 ? res.height : 1080;

            // Convert raw pixels to editor points
            float editorW = screenW / ppp;
            float editorH = screenH / ppp;

            // Define target size (half screen, constrained by minSize)
            float targetW = Mathf.Max(window.minSize.x, editorW / 2f);
            float targetH = Mathf.Max(window.minSize.y, editorH / 2f);

            // Calculate position for centering
            float x = (editorW - targetW) * 0.5f;
            float y = (editorH - targetH) * 0.5f;

            // Apply position and size
            window.position = new Rect(x, y, targetW, targetH);
        }
    }
}