using KissMyAssets.VisualNovelCore.Runtime;
using System;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Defines the contract for controlling node interactions, connections, and structural changes
    /// within the dialogue editor's graph view. This acts as the bridge between node views and the data/canvas controllers.
    /// </summary>
    public interface INodeConnectionController
    {
        /// <summary>
        /// Action invoked when a user requests to set a specific <see cref="DialogueStartNodeData"/> as the official entry point for the dialogue.
        /// </summary>
        public Action<DialogueStartNodeData> OnMakeStart { get; set; }

        /// <summary>
        /// Action invoked when a node's output port is clicked, initiating a connection. 
        /// The value is a callback function that will be executed when the user selects a target node.
        /// </summary>
        public Action<BaseNodeEditorView> OnLink { get; set; }

        /// <summary>
        /// Retrieves the visual representation (view) of a connected node by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the target node.</param>
        /// <returns>The <see cref="BaseNodeEditorView"/> instance for the given ID, or null if not found.</returns>
        public BaseNodeEditorView GetConnectedNodeById(Guid id);

        /// <summary>
        /// Requests the permanent deletion of a node from both the graph view and the underlying dialogue data.
        /// </summary>
        /// <param name="id">The unique ID of the node to delete.</param>
        public void RequestDelete(Guid id);
    }
}