using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Controller responsible for drawing and managing the left-hand toolbar panel 
    /// in the dialogue graph editor, primarily used for creating new nodes.
    /// </summary>
    public class DialogueSceneGraphToolbarController
    {
        private readonly EditorWindow _parentWindow;
        private readonly DialogueSceneGraphDataController _dataController;
        private readonly DialogueSceneGraphCanvasController _canvasController;

        private Vector2 _leftScroll;
        private EDialogueNodeType _selectedNodeType = EDialogueNodeType.Replica;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueSceneGraphToolbarController"/> class.
        /// </summary>
        /// <param name="parentWindow">The containing Unity <see cref="EditorWindow"/>.</param>
        /// <param name="dataController">The controller managing the node data.</param>
        /// <param name="canvasController">The controller managing the canvas (used to determine the creation position).</param>
        public DialogueSceneGraphToolbarController(EditorWindow parentWindow, DialogueSceneGraphDataController dataController, DialogueSceneGraphCanvasController canvasController)
        {
            _parentWindow = parentWindow;
            _dataController = dataController;
            _canvasController = canvasController;
        }

        /// <summary>
        /// Main drawing method for the toolbar. Draws the content within the specified rectangle area.
        /// </summary>
        /// <param name="rect">The screen rectangle defining the drawing area for the toolbar.</param>
        public void DrawTools(Rect rect)
        {
            GUILayout.BeginArea(rect, EditorStyles.helpBox);
            _leftScroll = EditorGUILayout.BeginScrollView(_leftScroll);

            DrawNodeCreationSection();

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the main section of the toolbar responsible for node creation controls.
        /// </summary>
        private void DrawNodeCreationSection()
        {
            GUILayout.Label("Nodes", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            DrawNodeTypeSelection();
            DrawCreateButton();
        }

        /// <summary>
        /// Draws the EnumPopup control for selecting the type of node to create.
        /// </summary>
        private void DrawNodeTypeSelection()
        {
            // EnumPopup for selecting the node type (e.g., Replica, Choice, Start, End)
            _selectedNodeType = (EDialogueNodeType)EditorGUILayout.EnumPopup(
                new GUIContent("Node Type", "Type of the node to be created"),
                _selectedNodeType
            );

            EditorGUILayout.Space(4);
        }

        /// <summary>
        /// Draws the button that triggers the creation of a new node.
        /// </summary>
        private void DrawCreateButton()
        {
            if (GUILayout.Button(new GUIContent("Create", "Add a new node of the selected type"), GUILayout.Height(26)))
            {
                // Create the new node data and view at the center of the current canvas view
                _dataController.CreateNewNode(_selectedNodeType, _canvasController.GetViewCenter());

                _parentWindow.Repaint();
            }
        }
    }
}