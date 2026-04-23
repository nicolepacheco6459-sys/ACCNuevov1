using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Controller responsible for managing the state and display of the Dialogue Scene Graph Editor Window.
    /// It handles opening the window, ensuring its proper size, and centering it on the screen.
    /// </summary>
    public class DialogueSceneGraphWindowController
    {
        private DialogueSceneGraphWindow _currentWindow = null;

        /// <summary>
        /// Opens and initializes the Dialogue Scene Graph editor window with the provided dialogue data.
        /// If the window is already open, it brings it into focus.
        /// </summary>
        /// <param name="dialogData">The <see cref="DialogueData"/> asset to be edited.</param>
        /// <param name="sceneName">The name to be displayed on the window title (usually the scene name).</param>
        public void ShowWindow(DialogueData dialogData, string sceneName)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Focus();
                return;
            }

            // Create or get the existing window instance
            _currentWindow = EditorWindow.GetWindow<DialogueSceneGraphWindow>(true, sceneName, true);
            _currentWindow.minSize = new Vector2(360, 240);

            CalculateAndSetWindowPosition();

            _currentWindow.OnOpen(dialogData);
            _currentWindow.Show();
        }

        /// <summary>
        /// Calculates the optimal size and position to center the window on the main screen, 
        /// respecting the window's minimum size.
        /// </summary>
        private void CalculateAndSetWindowPosition()
        {
            // Get screen resolution and DPI scale
            var res = Screen.currentResolution;
            float ppp = EditorGUIUtility.pixelsPerPoint;

            // Fallback screen dimensions if resolution is not available
            int screenW = res.width > 0 ? res.width : 1920;
            int screenH = res.height > 0 ? res.height : 1080;

            // Convert raw pixels to editor points
            float editorW = screenW / ppp;
            float editorH = screenH / ppp;

            // Define target size (max of min size and screen size)
            float targetW = Mathf.Max(_currentWindow.minSize.x, editorW);
            float targetH = Mathf.Max(_currentWindow.minSize.y, editorH);

            // Calculate position for centering
            float x = (editorW - targetW) * 0.5f;
            float y = (editorH - targetH) * 0.5f;

            // Apply position and size
            _currentWindow.position = new Rect(x, y, targetW, targetH);
        }
    }
}