using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueData
    {
        [field: SerializeField] public DialogueStartNodeData StartNode { get; set; }

        [field: SerializeReference]
        public List<BaseDialogueNodeData> Nodes { get; set; } = new List<BaseDialogueNodeData>();
    }
}