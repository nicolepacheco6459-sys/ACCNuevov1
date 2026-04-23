using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public class DialogueSceneGraphWindow: EditorWindow
    {

        private DialogueSceneGraphToolbarController _toolbarController;
        private DialogueSceneGraphCanvasController _canvasController;
        private DialogueSceneGraphDataController _dataController;

        public void OnOpen(DialogueData data)
        {
            _dataController = new DialogueSceneGraphDataController(data);
            _canvasController = new DialogueSceneGraphCanvasController(this, _dataController);
            _toolbarController = new DialogueSceneGraphToolbarController(this, _dataController, _canvasController);

            Repaint();
        }

        private void OnGUI()
        {
            DrawSplitLayout();
        }

        private void DrawSplitLayout()
        {
            var full = new Rect(0, 0, position.width, position.height);
            var left = new Rect(full.x, full.y, DialogueSceneGraphConstants.LeftPanelWidth, full.height);
            var right = new Rect(left.xMax, full.y, full.width - DialogueSceneGraphConstants.LeftPanelWidth, full.height);

            _canvasController.DrawCanvas(right);
            _toolbarController.DrawTools(left);
        }
    }
}