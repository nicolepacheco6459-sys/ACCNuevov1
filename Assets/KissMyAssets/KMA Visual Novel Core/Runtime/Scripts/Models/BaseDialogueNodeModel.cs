using Cysharp.Threading.Tasks;
using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public abstract class BaseDialogueNodeModel
    {
        public Guid NodeId { get; }
        public BackgroundHolderModel BackgroundHolder { get; }
        public ActorsHolderModel ActorsHolder { get; }

        public BaseDialogueNodeModel(BaseDialogueNodeData data)
        {
            NodeId = data.NodeId;
            BackgroundHolder = new(data.BackgroundHolder);
            ActorsHolder = new(data.ActorsHolder);
        }

        public abstract Guid GetNextNode();

        public virtual async UniTask AcceptView(IDialogueView nodeView)
        {
            await nodeView.TryToChangeBackground(BackgroundHolder);
            await nodeView.TryToChageActors(ActorsHolder);
        }
    }
}
