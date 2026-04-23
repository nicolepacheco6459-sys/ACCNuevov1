using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueEndNodeData : BaseDialogueNodeData
    {
        public override EDialogueNodeType NodeType => EDialogueNodeType.End;

        public override BaseDialogueNodeModel CreateModel()
        {
            return new EndNodeModel(this);
        }
    }
}