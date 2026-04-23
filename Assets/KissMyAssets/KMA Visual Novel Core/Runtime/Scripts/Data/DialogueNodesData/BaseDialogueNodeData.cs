using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public abstract class BaseDialogueNodeData
    {
        public virtual EDialogueNodeType NodeType { get; set; }
        [field: SerializeField] public SerializableGuid NodeId { get; set; }
        [field: SerializeField] public BackgroundHolderData BackgroundHolder { get; set; }
        [field: SerializeReference] public ActorsHolderData ActorsHolder { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; } = Vector2.zero;

        public BaseDialogueNodeData()
        {
            NodeId = Guid.NewGuid();
            ActorsHolder = new ActorsHolderData();
            BackgroundHolder = new BackgroundHolderData();
        }

        public abstract BaseDialogueNodeModel CreateModel();
    }
}