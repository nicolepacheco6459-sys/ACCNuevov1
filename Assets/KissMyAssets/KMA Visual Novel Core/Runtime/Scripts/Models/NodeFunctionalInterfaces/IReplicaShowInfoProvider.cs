namespace KissMyAssets.VisualNovelCore.Runtime
{
    public interface IReplicaShowInfoProvider
    {
        public ActorsHolderModel ReplicaActorsHolder { get; }
        public DialogueTextModel ReplicaDialogueText { get; }
    }
}
