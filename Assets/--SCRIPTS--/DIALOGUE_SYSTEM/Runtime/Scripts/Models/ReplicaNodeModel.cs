using Cysharp.Threading.Tasks;
using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ReplicaNodeModel : BaseDialogueNodeModel, IReplicaShowInfoProvider
    {
        private Guid _nextNode;
        public DialogueTextModel DialogueText { get; }

        public ActorsHolderModel ReplicaActorsHolder => ActorsHolder;

        public DialogueTextModel ReplicaDialogueText => DialogueText;

        public ReplicaNodeModel(DialogueReplicaNodeData data) : base(data)
        {
            _nextNode = data.NextNode;
            DialogueText = new DialogueTextModel(data.Text);
        }

        public override Guid GetNextNode()
        {
            return _nextNode;
        }

        public override async UniTask AcceptView(IDialogueView nodeView)
        {
            await base.AcceptView(nodeView);

            await nodeView.ShowReplica(this);
        }
    }
}
