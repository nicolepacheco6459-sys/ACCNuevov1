using KissMyAssets.VisualNovelCore.Runtime;
using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public class EmptyNodeEditorView : BaseNodeEditorView
    {
        private readonly DialogueEmptyNodeData _data;
        private BaseNodeEditorView _nextNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyNodeEditorView"/> class.
        /// </summary>
        /// <param name="data">The underlying dialogue node data, expected to be <see cref="DialogueEmptyNodeData"/>.</param>
        /// <param name="connectionController">The controller for handling node connections.</param>
        public EmptyNodeEditorView(BaseDialogueNodeData data, INodeConnectionController connectionController) : base(data, connectionController)
        {
            _data = data as DialogueEmptyNodeData;
        }

        // === DRAWING ===

        /// <summary>
        /// Draws the node's output connection to the next node.
        /// </summary>
        public override void DrawConnection()
        {
            if (_nextNode == null)
                TryToReloadConnection();

            DrawConnectionTo(_nextNode);
        }

        /// <summary>
        /// Draws the node title, the connection input button (left edge), the connection output button (right edge), and the delete button (top-right corner).
        /// </summary>
        /// <param name="r">The rectangle representing the node window.</param>
        protected override void DrawChrome(Rect r)
        {
            // Input port for incoming connections
            ChromeEdgeButtonLeft("@", r, () => ConnectionController.OnLink.Invoke(this));
            // Output port for the single outgoing connection, sets LinkToNextNode as the target action
            ChromeEdgeButtonRight(">", r, () => ConnectionController.OnLink = LinkToNextNode);
            // Delete node button
            ChromeCornerButtonTopRight("X", r, () => ConnectionController.RequestDelete(NodeId));
        }

        /// <summary>
        /// Draws the main body content, including controls for background, actors.
        /// </summary>
        protected override void DrawBody()
        {
            if (_data != null)
            {
                PartBackgroundDropdown(_data.BackgroundHolder);
                PartActorsList(_data.ActorsHolder);
            }
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
        /// using the stored <see cref="DialogueReplicaNodeData.NextNode"/> ID.
        /// </summary>
        /// <returns>True if the connection was successfully reloaded and a target node view was found; otherwise, false.</returns>
        private bool TryToReloadConnection()
        {
            _nextNode = ConnectionController.GetConnectedNodeById(_data.NextNode);
            return _nextNode != null;
        }

        /// <summary>
        /// Responds to a node deletion event. If the deleted node was the target of this node's outgoing connection,
        /// the connection is broken and the <see cref="DialogueReplicaNodeData.NextNode"/> is reset.
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
    }
}
