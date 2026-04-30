using Cysharp.Threading.Tasks;
using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class StartNodeModel : BaseDialogueNodeModel
    {
        private Guid _nextNode;
        public StartNodeModel(DialogueStartNodeData data) : base(data)
        {
            _nextNode = data.NextNode;
        }

        public override Guid GetNextNode()
        {
            return _nextNode;
        }

        public override async UniTask AcceptView(IDialogueView nodeView) => await UniTask.CompletedTask;
    }
}
