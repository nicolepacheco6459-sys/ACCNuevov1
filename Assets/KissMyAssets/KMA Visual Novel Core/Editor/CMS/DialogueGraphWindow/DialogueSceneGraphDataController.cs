using KissMyAssets.VisualNovelCore.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Controller responsible for managing the dialogue graph's data layer, including 
    /// creating, deleting, and updating nodes, as well as handling connections between them.
    /// It implements the <see cref="INodeConnectionController"/> interface to serve as a bridge for node views.
    /// </summary>
    public class DialogueSceneGraphDataController : INodeConnectionController
    {
        private readonly DialogueData _dialogData;
        private readonly Dictionary<Guid, BaseNodeEditorView> _nodeViews = new();

        /// <inheritdoc/>
        public Action<BaseNodeEditorView> OnLink { get; set; }

        /// <inheritdoc/>
        public Action<DialogueStartNodeData> OnMakeStart { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueSceneGraphDataController"/> class.
        /// </summary>
        /// <param name="data">The root <see cref="DialogueData"/> object to manage.</param>
        public DialogueSceneGraphDataController(DialogueData data)
        {
            _dialogData = data;

            // Load existing nodes from data
            foreach (var nodeData in _dialogData.Nodes)
            {
                TryToCreateNodeView(nodeData);
            }

            // Set up event handlers
            OnMakeStart = TryToSetStarterNode;
        }

        /// <summary>
        /// Iterates through and draws all node views, ensuring connection and start node validity checks are performed.
        /// </summary>
        public void DrawNodes()
        {
            ValidateStartNodes();

            for (int i = 0; i < _dialogData.Nodes.Count; i++)
            {
                Guid nodeId = _dialogData.Nodes[i].NodeId;

                if (_nodeViews.TryGetValue(nodeId, out var nodeView))
                {
                    nodeView.DrawNode();
                    nodeView.DrawConnection();
                }
            }
        }

        /// <summary>
        /// Creates a new dialogue node and its corresponding view at a specified position.
        /// </summary>
        /// <param name="type">The type of the dialogue node to create.</param>
        /// <param name="position">The canvas position for the new node.</param>
        public void CreateNewNode(EDialogueNodeType type, Vector2 position)
        {
            if (!EditorViewConstructors.NodeDataCreator.TryGetValue(type, out var dataCreator))
                return;

            BaseDialogueNodeData newData = CreateNodeData(dataCreator, position);

            _dialogData.Nodes.Add(newData);

            if (newData is DialogueStartNodeData startData)
            {
                // Automatically designate a new Start Node as the starter if one is created
                TryToSetStarterNode(startData);
            }

            TryToCreateNodeView(newData);
        }

        /// <summary>
        /// Helper method to create the node data and set its initial position.
        /// </summary>
        private BaseDialogueNodeData CreateNodeData(Func<BaseDialogueNodeData> dataCreator, Vector2 position)
        {
            var newData = dataCreator();
            newData.Position = position;
            return newData;
        }

        /// <summary>
        /// Sets the provided Start Node as the official dialogue entry point in the root data.
        /// </summary>
        private void TryToSetStarterNode(DialogueStartNodeData startData)
        {
            _dialogData.StartNode = startData;
        }

        /// <summary>
        /// Iterates through all Start Nodes and updates their internal state based on whether they are 
        /// currently set as the official dialogue starter.
        /// </summary>
        private void ValidateStartNodes()
        {
            foreach (var nodeView in _nodeViews.Values)
            {
                if (nodeView is StartNodeEditorView startNodeView)
                {
                    // Update the view's internal flag
                    startNodeView.MakeDialogStart(startNodeView.NodeId == _dialogData.StartNode.NodeId);
                }
            }
        }

        /// <summary>
        /// Creates the visual editor view for a given dialogue node data object and adds it to the view dictionary.
        /// </summary>
        /// <param name="data">The underlying dialogue node data.</param>
        /// <returns>True if the view was successfully created and added; otherwise, false.</returns>
        private bool TryToCreateNodeView(BaseDialogueNodeData data)
        {
            if (!EditorViewConstructors.NodeViewCreator.TryGetValue(data.NodeType, out var viewCreator))
                return false;

            var view = viewCreator(data, this);

            Debug.Log($"[DialogueSceneGraphDataController] Created node view: {data.NodeType}");

            _nodeViews.Add(data.NodeId, view);

            return true;
        }

        /// <inheritdoc/>
        public BaseNodeEditorView GetConnectedNodeById(Guid id)
        {
            if (id == Guid.Empty || !_nodeViews.ContainsKey(id))
                return null;

            return _nodeViews[id];
        }

        /// <inheritdoc/>
        public void RequestDelete(Guid nodeId)
        {
            RemoveNodeFromData(nodeId);
            RemoveNodeView(nodeId);
            NotifyNodesOfDeletion(nodeId);
        }

        /// <summary>
        /// Removes the node data object from the root dialogue data list.
        /// </summary>
        private void RemoveNodeFromData(Guid nodeId)
        {
            for (int i = _dialogData.Nodes.Count - 1; i >= 0; i--)
            {
                if (_dialogData.Nodes[i].NodeId == nodeId)
                {
                    _dialogData.Nodes.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes the visual node view from the controller's internal dictionary.
        /// </summary>
        private void RemoveNodeView(Guid nodeId)
        {
            _nodeViews.Remove(nodeId);
        }

        /// <summary>
        /// Notifies all remaining node views that a node has been deleted, allowing them to break any outgoing connections to it.
        /// </summary>
        private void NotifyNodesOfDeletion(Guid nodeId)
        {
            foreach (var nodeView in _nodeViews.Values)
            {
                nodeView.BrokeConnection(nodeId);
            }
        }
    }
}