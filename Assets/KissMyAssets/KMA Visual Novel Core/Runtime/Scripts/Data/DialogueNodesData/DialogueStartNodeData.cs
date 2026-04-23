using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueStartNodeData : BaseDialogueNodeData
    {
        public override EDialogueNodeType NodeType => EDialogueNodeType.Start;

        [field: SerializeField] public SerializableGuid NextNode { get; set; }

        public override BaseDialogueNodeModel CreateModel()
        {
            return new StartNodeModel(this);
        }
    }
}