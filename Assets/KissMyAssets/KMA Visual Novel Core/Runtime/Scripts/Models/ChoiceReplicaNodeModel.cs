using Cysharp.Threading.Tasks;
using System;
using System.Linq;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ChoiceReplicaNodeModel : BaseDialogueNodeModel, IReplicaShowInfoProvider, IChoiceShowInfoProvider
    {
        private Guid _nextNode;
        public DialogueTextModel DialogueText { get; }
        public ChoiceOptionsHolderModel OptionsHolder { get; private set; }

        public ActorsHolderModel ReplicaActorsHolder => ActorsHolder;
        public DialogueTextModel ReplicaDialogueText => DialogueText;
        public ChoiceOptionsHolderModel ChoiceOptionsHolder => OptionsHolder;

        public ChoiceReplicaNodeModel(DialogueChoiceReplicaNodeData data) : base(data)
        {
            DialogueText = new DialogueTextModel(data.Text);
            OptionsHolder = new ChoiceOptionsHolderModel(data.Options.Select(data => new ChoiceOptionModel(data)).ToList());
        }
        public override Guid GetNextNode()
        {
            return _nextNode;
        }

        public override async UniTask AcceptView(IDialogueView nodeView)
        {
            await base.AcceptView(nodeView);

            await nodeView.ShowReplica(this);

            Guid result = await nodeView.ShowChoice(this, true);
            SetNextNode(result);
        }

        private void SetNextNode(Guid chosenNextNode)
        {
            _nextNode = chosenNextNode;
        }
    }
}