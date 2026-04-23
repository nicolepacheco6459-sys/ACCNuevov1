using System.Collections.Generic;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ChoiceOptionsHolderModel
    {
        public IReadOnlyList<ChoiceOptionModel> Options { get; } = new List<ChoiceOptionModel>();

        public ChoiceOptionsHolderModel(List<ChoiceOptionModel> options)
        {
            Options = options;
        }
    }
}
