using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Controller responsible for managing the visual canvas area of the dialogue graph.
    /// It handles panning, zooming, and applying the transformation matrix for rendering nodes.
    /// </summary>
    public class DialogueSceneGraphCanvasController
    {
        private readonly EditorWindow _parentWindow;
        private readonly DialogueSceneGraphDataController _dataController;

        private Vector2 _pan = Vector2.zero;
        private float _zoom = 1f;
        private Vector2 _zoomPivot = Vector2.zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueSceneGraphCanvasController"/> class.
        /// </summary>
        /// <param name="parentWindow">The containing Unity <see cref="EditorWindow"/>.</param>
        /// <param name="dataController">The controller managing the node data and views.</param>
        public DialogueSceneGraphCanvasController(EditorWindow parentWindow, DialogueSceneGraphDataController dataController)
        {
            _parentWindow = parentWindow;
            _dataController = dataController;
        }

        /// <summary>
        /// Main drawing method for the canvas. It applies the zoom/pan transformation matrix and draws all nodes.
        /// </summary>
        /// <param name="area">The screen rectangle defining the drawing area for the canvas.</param>
        public void DrawCanvas(Rect area)
        {
            HandleCanvasEvents(area);

            // Apply the pan/zoom matrix only to the canvas area
            Matrix4x4 old = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(_pan + _zoomPivot, Quaternion.identity, Vector3.one * _zoom);

            // Nodes draw themselves using world coordinates within the transformed matrix
            _parentWindow.BeginWindows();
            _dataController.DrawNodes();
            _parentWindow.EndWindows();

            GUI.matrix = old; // Restore the original matrix
        }

        /// <summary>
        /// Handles all interactive events within the canvas area, such as panning and zooming.
        /// </summary>
        /// <param name="area">The canvas rectangle.</param>
        private void HandleCanvasEvents(Rect area)
        {
            var e = Event.current;
            // Mouse position is local to the editor window
            Vector2 mouse = e.mousePosition;

            HandlePanning(e, mouse);
            HandleZooming(e, mouse);
        }

        /// <summary>
        /// Handles mouse drag events for panning (moving the canvas view).
        /// Panning is triggered by Middle Mouse Button (MMB) or Ctrl/Cmd + Left Mouse Button.
        /// </summary>
        private void HandlePanning(Event e, Vector2 mouse)
        {
            // Panning keys: MMB or Ctrl/Cmd + LMB
            bool panKey = e.button == 2 || (e.button == 0 && e.control);
            if (e.type == EventType.MouseDrag && panKey)
            {
                _pan += e.delta;
                e.Use();
                _parentWindow.Repaint();
            }
        }

        /// <summary>
        /// Handles mouse scroll events for zooming in and out.
        /// </summary>
        private void HandleZooming(Event e, Vector2 mouse)
        {
            if (e.type == EventType.ScrollWheel)
            {
                float old = _zoom;
                // Clamp zoom level to defined constants
                _zoom = Mathf.Clamp(_zoom - e.delta.y * 0.01f, DialogueSceneGraphConstants.ZoomMin, DialogueSceneGraphConstants.ZoomMax);

                // Adjust pan to zoom towards the mouse cursor (pivot point)
                _zoomPivot -= (mouse - _zoomPivot) * (1f - _zoom / old);

                e.Use();
                _parentWindow.Repaint();
            }
        }

        /// <summary>
        /// Calculates the world-space position of the center of the current view (useful for placing new nodes).
        /// </summary>
        /// <returns>The world-space coordinates of the canvas center.</returns>
        public Vector2 GetViewCenter()
        {

            float canvasW = _parentWindow.position.width - DialogueSceneGraphConstants.LeftPanelWidth;
            float canvasH = _parentWindow.position.height;

            Vector2 localCenter = new Vector2(canvasW * 0.5f, canvasH * 0.5f);
            Vector2 worldCenter = LocalToWorld(localCenter);

            return worldCenter;
        }
        private Vector2 LocalToWorld(Vector2 p) => (p - _zoomPivot - _pan) / _zoom;
    }
}