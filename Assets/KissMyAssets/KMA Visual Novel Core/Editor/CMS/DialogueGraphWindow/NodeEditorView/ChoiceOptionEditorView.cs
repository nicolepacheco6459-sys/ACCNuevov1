using KissMyAssets.VisualNovelCore.Runtime;
using System;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
        /// <summary>
        /// Represents the visual and interactive element for a single choice option within a Choice Node.
        /// It handles drawing the option's text area, remove button, connection button, and the connection line itself.
        /// </summary>
        public class ChoiceOptionEditorView
        {
            private ChoiceOptionData _data;
            private BaseNodeEditorView _nextNode;

            private INodeConnectionController _connectionController;

            private Vector2 _worldConnectAnchor;

            private Rect _localBlockRect;

            /// <summary>
            /// The measured height of the option block in the last frame.
            /// </summary>
            public float LastMeasuredHeight { get; private set; }

            /// <summary>
            /// The available width for the body of the option block, calculated by the parent node.
            /// </summary>
            public float BodyWidth { get; set; }

            /// <summary>
            /// The world space offset of the parent node's body area. Used to calculate the connection anchor position.
            /// </summary>
            public Vector2 BodyOffsetWorld { get; set; }

            /// <summary>
            /// Event fired when the user requests to remove this option.
            /// </summary>
            public event Action OnRemoveRequested;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChoiceOptionEditorView"/> class.
            /// </summary>
            /// <param name="choiceOptionData">The data model for this choice option.</param>
            /// <param name="nodeConnectionController">The controller for managing node connections.</param>
            public ChoiceOptionEditorView(ChoiceOptionData choiceOptionData, INodeConnectionController nodeConnectionController)
            {
                _data = choiceOptionData;
                _connectionController = nodeConnectionController;
            }


            /// <summary>
            /// Draws the choice option block (remove button, text area, connect button) or measures its required size.
            /// This method performs two roles based on the <paramref name="isMeasuring"/> flag: calculating the block's height 
            /// and drawing the interactive GUI controls.
            /// </summary>
            /// <param name="isMeasuring">If true, only measures the size and returns; otherwise, draws the GUI controls.</param>
            public void DrawOptionBlock(bool isMeasuring)
            {
                // 1. Calculate required sizes for elements
                var removeContent = new GUIContent("✕");
                var connectContent = new GUIContent(">");

                Vector2 removeSize = GUI.skin.button.CalcSize(removeContent);
                Vector2 connectSize = GUI.skin.button.CalcSize(connectContent);

                // Calculate width for the text area
                float textWidth = Mathf.Max(DialogueNodeConstants.TextAreaMinWidth, BodyWidth - (removeSize.x + connectSize.x + DialogueNodeConstants.ControlGapX * 2f));

                // Calculate height for the text area based on content
                float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(_data.Text ?? string.Empty), textWidth);
                textHeight = Mathf.Max(textHeight, 2 * EditorGUIUtility.singleLineHeight); // Ensure minimum height of two lines

                // Determine the total block height, which is the maximum height of its controls
                float blockHeight = Mathf.Max(textHeight, Mathf.Max(removeSize.y, connectSize.y, EditorGUIUtility.singleLineHeight));
                LastMeasuredHeight = blockHeight;

                // 2. Handle Measurement Phase
                if (isMeasuring)
                    return;

                // 3. Draw Phase: Reserve space and calculate control positions
                _localBlockRect = GUILayoutUtility.GetRect(BodyWidth, blockHeight, GUILayout.ExpandWidth(false));

                // Calculate Rects for individual controls in the reserved space
                Rect removeRect = new Rect(_localBlockRect.x, _localBlockRect.y, removeSize.x, blockHeight);
                Rect connectRect = new Rect(_localBlockRect.x + _localBlockRect.width - connectSize.x, _localBlockRect.y, connectSize.x, blockHeight);
                Rect textRect = new Rect(
                    removeRect.xMax + DialogueNodeConstants.ControlGapX,
                    _localBlockRect.y,
                    Mathf.Max(60f, connectRect.xMin - DialogueNodeConstants.ControlGapX - (removeRect.xMax + DialogueNodeConstants.ControlGapX)),
                    blockHeight
                );

                // 4. Handle interactions and drawing
                if (GUI.Button(removeRect, removeContent))
                    OnRemoveRequested?.Invoke();

                if (GUI.Button(connectRect, connectContent))
                    _connectionController.OnLink = LinkOption; // Set the function to be called when the user selects a target node

                // Text Area
                EditorGUI.BeginChangeCheck();
                string newText = EditorGUI.TextArea(textRect, _data.Text ?? string.Empty, EditorStyles.textArea);
                if (EditorGUI.EndChangeCheck())
                    _data.Text = newText;

                // 5. Update Connection Anchor Position on Repaint
                if (Event.current.type == EventType.Repaint)
                {
                    // Anchor is the center of the connect button, in world space (relative to the editor window)
                    _worldConnectAnchor = BodyOffsetWorld + connectRect.center;
                }
            }

            /// <summary>
            /// Sets the target node for this option and updates the underlying data.
            /// This method is called when the user successfully links this option to another node.
            /// </summary>
            /// <param name="target">The target node view to link to.</param>
            public void LinkOption(BaseNodeEditorView target)
            {
                if (target == null)
                    return;

                _nextNode = target;
                _data.RelatedNodeId = target.NodeId;
            }

            /// <summary>
            /// Draws the connection line from this option's connection anchor to the target node.
            /// Attempts to reload the connection if the target node view is missing (e.g., after loading the editor).
            /// </summary>
            public void DrawConnection()
            {
                if (_nextNode == null && !TryToReloadConnection())
                {
                    return;
                }

                // Connection starts from the center of the connect button
                Vector3 start = new Vector3(_worldConnectAnchor.x, _worldConnectAnchor.y, 0f);

                // Connection ends at the center of the left edge of the target node
                Rect toRect = new Rect(_nextNode.Position, _nextNode.CachedSize);
                Vector3 end = new Vector3(toRect.xMin, toRect.center.y, 0f);

                ConnectionDrawer.DrawConnectionTo(start, end);
            }

            /// <summary>
            /// Responds to a node deletion event. If the deleted node was the target of this option,
            /// the connection is broken and the <see cref="ChoiceOptionData.RelatedNodeId"/> is reset.
            /// </summary>
            /// <param name="deletedNodeId">The unique ID of the node that was deleted.</param>
            public void BrokeConnection(Guid deletedNodeId)
            {
                if (_data.RelatedNodeId != deletedNodeId)
                    return;

                _nextNode = null;
                _data.RelatedNodeId = Guid.Empty;
            }

            /// <summary>
            /// Attempts to retrieve the visual representation (<see cref="BaseNodeEditorView"/>) of the connected target node 
            /// using the stored <see cref="ChoiceOptionData.RelatedNodeId"/>. 
            /// This is crucial for re-establishing the view reference after the editor loads.
            /// </summary>
            /// <returns>True if the connection was successfully reloaded and a target node view was found; otherwise, false.</returns>
            private bool TryToReloadConnection()
            {
                _nextNode = _connectionController.GetConnectedNodeById(_data.RelatedNodeId);

                return _nextNode != null;
            }
        }
}