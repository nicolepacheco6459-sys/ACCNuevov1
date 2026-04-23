using Cysharp.Threading.Tasks;
using System;
using System.Linq;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ChoiceNodeModel : BaseDialogueNodeModel, IChoiceShowInfoProvider
    {
        private Guid _nextNode;
        public ChoiceOptionsHolderModel OptionsHolder { get; private set; }

        public ChoiceOptionsHolderModel ChoiceOptionsHolder => OptionsHolder;

        public ChoiceNodeModel(DialogueChoiceNodeData data) : base(data)
        {
            OptionsHolder = new ChoiceOptionsHolderModel(data.Options.Select(data => new ChoiceOptionModel(data)).ToList());
        }

        public override Guid GetNextNode()
        {
            return _nextNode;
        }
        public override async UniTask AcceptView(IDialogueView nodeView)
        {
            await base.AcceptView(nodeView);

            Guid result = await nodeView.ShowChoice(this);
            SetNextNode(result);
        }

        private void SetNextNode(Guid chosenNextNode)
        {
            _nextNode = chosenNextNode;
        }
    }
}
