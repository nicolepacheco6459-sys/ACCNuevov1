using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ChoiceOptionModel
    {
        public Guid RelatedNodeId { get; }
        public DialogueTextModel DialogueText { get; }

        public ChoiceOptionModel(ChoiceOptionData data)
        {
            DialogueText = new DialogueTextModel(data.Text);
            RelatedNodeId = data.RelatedNodeId;
        }
    }
}