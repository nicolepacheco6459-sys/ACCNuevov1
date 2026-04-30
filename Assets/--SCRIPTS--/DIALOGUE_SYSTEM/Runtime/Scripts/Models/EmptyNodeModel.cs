using Cysharp.Threading.Tasks;
using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class EmptyNodeModel : BaseDialogueNodeModel
    {
        private Guid _nextNode;

        public EmptyNodeModel(DialogueEmptyNodeData data) : base(data)
        {
            _nextNode = data.NextNode;
        }

        public override Guid GetNextNode()
        {
            return _nextNode;
        }

        public override async UniTask AcceptView(IDialogueView nodeView)
        {
            await base.AcceptView(nodeView);
            await nodeView.WaitForSkip();
        }
    }
}
