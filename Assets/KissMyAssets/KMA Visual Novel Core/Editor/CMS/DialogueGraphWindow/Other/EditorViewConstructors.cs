using KissMyAssets.VisualNovelCore.Runtime;
using System;
using System.Collections.Generic;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public static class EditorViewConstructors
    {
        public static readonly Dictionary<EDialogueNodeType, Func<BaseDialogueNodeData, INodeConnectionController, BaseNodeEditorView>> NodeViewCreator = new()
        {
            { EDialogueNodeType.Start, (data, connection) =>  new StartNodeEditorView(data, connection) },
            { EDialogueNodeType.End, (data, connection) =>  new EndNodeEditorView(data, connection) },
            { EDialogueNodeType.Replica, (data, connection) => new ReplicaNodeEditorView(data, connection) },
            { EDialogueNodeType.Choice, (data, connection) =>  new ChoiceNodeEditorView(data, connection) },
            { EDialogueNodeType.ChoiceReplica, (data, connection) =>  new ChoiceReplicaNodeEditorView(data, connection) },
            { EDialogueNodeType.Empty, (data, connection) =>  new EmptyNodeEditorView(data, connection) },
        };

        public static readonly Dictionary<EDialogueNodeType, Func<BaseDialogueNodeData>> NodeDataCreator = new()
        {
            { EDialogueNodeType.Start, () => new DialogueStartNodeData() },
            { EDialogueNodeType.End, () => new DialogueEndNodeData() },
            { EDialogueNodeType.Replica, () => new DialogueReplicaNodeData() },
            { EDialogueNodeType.Choice, () => new DialogueChoiceNodeData() },
            { EDialogueNodeType.ChoiceReplica, () => new DialogueChoiceReplicaNodeData() },
            { EDialogueNodeType.Empty, () => new DialogueEmptyNodeData() },
        };
    }
}
