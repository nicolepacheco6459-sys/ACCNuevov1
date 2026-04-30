using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueChoiceReplicaNodeData : BaseDialogueNodeData
    {
        public override EDialogueNodeType NodeType => EDialogueNodeType.ChoiceReplica;

        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<ChoiceOptionData> Options { get; set; } = new List<ChoiceOptionData>();

        public override BaseDialogueNodeModel CreateModel()
        {
            return new ChoiceReplicaNodeModel(this);
        }
    }
}
