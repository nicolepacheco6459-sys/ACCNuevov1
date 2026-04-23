using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueEmptyNodeData : BaseDialogueNodeData
    {
        [field: SerializeField] public SerializableGuid NextNode { get; set; }
        public override EDialogueNodeType NodeType => EDialogueNodeType.Empty;

        public override BaseDialogueNodeModel CreateModel()
        {
            return new EmptyNodeModel(this);
        }
    }
}
