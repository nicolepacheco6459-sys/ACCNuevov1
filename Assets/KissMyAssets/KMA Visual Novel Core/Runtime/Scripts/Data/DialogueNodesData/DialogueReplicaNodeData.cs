using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueReplicaNodeData : BaseDialogueNodeData
    {
        public override EDialogueNodeType NodeType => EDialogueNodeType.Replica;

        [field: SerializeField] public SerializableGuid NextNode { get; set; }
        [field: SerializeField] public string Text { get; set; }

        public override BaseDialogueNodeModel CreateModel()
        {
            return new ReplicaNodeModel(this);
        }
    }
}