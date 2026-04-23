using KissMyAssets.VisualNovelCore.Runtime;
using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Editor view for the Dialogue Start Node (<see cref="DialogueStartNodeData"/>).
    /// This node marks the entry point of the dialogue and allows linking to the first content node.
    /// It also contains a control to mark the underlying data as the designated Start Node for the entire dialogue.
    /// </summary>
    public class StartNodeEditorView : BaseNodeEditorView
    {
        private DialogueStartNodeData _data;
        private BaseNodeEditorView _nextNode;
        private bool _isStartNode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartNodeEditorView"/> class.
        /// </summary>
        /// <param name="data">The underlying dialogue node data, expected to be <see cref="DialogueStartNodeData"/>.</param>
        /// <param name="connectionController">The controller for handling node connections.</param>
        public StartNodeEditorView(BaseDialogueNodeData data, INodeConnectionController connectionController) : base(data, connectionController)
        {
            _data = data as DialogueStartNodeData;
        }

        /// <summary>
        /// Marks the visual representation of the node as the designated start node of the dialogue.
        /// This influences the diagnostics panel.
        /// </summary>
        /// <param name="isStart">True if this node is the dialogue's starting node; otherwise, false.</param>
        public void MakeDialogStart(bool isStart)
        {
            _isStartNode = isStart;
        }

        // === DRAWING ===

        /// <summary>
        /// Draws the node title, the connection output button (right edge), and the delete button (top-right corner).
        /// </summary>
        /// <param name="r">The rectangle representing the node window.</param>
        protected override void DrawChrome(Rect r)
        {
            ChromeTitle("START", r);
            // Output port for the single outgoing connection
            ChromeEdgeButtonRight(">", r, () => ConnectionController.OnLink = LinkToNextNode);
            // Delete node button
            ChromeCornerButtonTopRight("X", r, () => ConnectionController.RequestDelete(NodeId));
        }

        /// <summary>
        /// Draws the main body content, which consists of a single button to designate this node as the dialogue's start.
        /// </summary>
        protected override void DrawBody()
        {
            // Button to set this node as the official start node in the dialogue manager
            PartButton("Make Start", () => ConnectionController.OnMakeStart.Invoke(_data));
        }

        /// <summary>
        /// Draws the node's output connection to the next node.
        /// </summary>
        public override void DrawConnection()
        {
            if (_nextNode == null)
                TryToReloadConnection();

            DrawConnectionTo(_nextNode);
        }

        // === DATA MANAGEMENT ===

        /// <summary>
        /// Updates the position of the underlying data when the node is dragged in the editor.
        /// </summary>
        /// <param name="position">The new position of the node.</param>
        protected override void OnNodePositionChanged(Vector2 position)
        {
            if (_data != null)
                _data.Position = position;
        }

        /// <summary>
        /// Sets the target node for the outgoing connection and updates the underlying data.
        /// This method is passed as a callback to the connection controller when the output port is clicked.
        /// </summary>
        /// <param name="node">The target node view to link to.</param>
        private void LinkToNextNode(BaseNodeEditorView node)
        {
            if (node == null || node == this)
                return;

            _nextNode = node;
            _data.NextNode = node.NodeId;
        }

        /// <summary>
        /// Attempts to retrieve the visual representation (<see cref="BaseNodeEditorView"/>) of the connected target node 
        /// using the stored <see cref="DialogueStartNodeData.NextNode"/> ID.
        /// </summary>
        /// <returns>True if the connection was successfully reloaded and a target node view was found; otherwise, false.</returns>
        private bool TryToReloadConnection()
        {
            _nextNode = ConnectionController.GetConnectedNodeById(_data.NextNode);
            return _nextNode != null;
        }

        /// <summary>
        /// Responds to a node deletion event. If the deleted node was the target of this node's outgoing connection,
        /// the connection is broken and the <see cref="DialogueStartNodeData.NextNode"/> is reset.
        /// </summary>
        /// <param name="deletedNodeId">The unique ID of the node that was deleted.</param>
        public override void BrokeConnection(Guid deletedNodeId)
        {
            if (_data != null && _data.NextNode == deletedNodeId)
            {
                _nextNode = null;
                _data.NextNode = Guid.Empty;
            }
        }

        // === DIAGNOSTICS ===

        /// <summary>
        /// Collects diagnostics for the start node, primarily checking if it is officially designated as the dialogue's starting point.
        /// </summary>
        protected override void CollectDiagnostics()
        {
            if (!_isStartNode)
            {
                Diagnostics.ErrorOnce("startnode.notdialogstart", $"Dialog Start Node (ID: {_data.NodeId}) is not set as the starting node in the dialog data.");
            }
        }
    }
}