using Cysharp.Threading.Tasks;
using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class EndNodeModel : BaseDialogueNodeModel
    {
        public EndNodeModel(DialogueEndNodeData data) : base(data) { }

        public override Guid GetNextNode()
        {
            return Guid.Empty;
        }
        public override async UniTask AcceptView(IDialogueView nodeView) => await UniTask.CompletedTask;
    }
}
