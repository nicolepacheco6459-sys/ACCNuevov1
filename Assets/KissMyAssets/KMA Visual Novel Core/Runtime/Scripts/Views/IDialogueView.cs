using Cysharp.Threading.Tasks;
using System;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Interface defining the presentation layer methods for displaying various dialogue node types.
    /// Used by the dialogue runner to interact with the UI.
    /// </summary>
    public interface IDialogueView
    {
        /// <summary>
        /// Asynchronously attempts to change the scene's background based on the provided model.
        /// </summary>
        /// <param name="backgroundModel">The model containing background information.</param>
        public UniTask TryToChangeBackground(BackgroundHolderModel backgroundModel);

        /// <summary>
        /// Asynchronously attempts to show/update actors on the screen based on the provided model.
        /// </summary>
        /// <param name="actorsModel">The model containing actor information.</param>
        public UniTask TryToChageActors(ActorsHolderModel actorsModel);

        /// <summary>
        /// Asynchronously displays a replica (line of dialogue) and waits for user interaction to proceed.
        /// </summary>
        /// <param name="showInfo">The model containing the replica data.</param>
        public UniTask ShowReplica(IReplicaShowInfoProvider showInfo);

        /// <summary>
        /// Asynchronously displays a choice menu and waits for the user to select an option.
        /// </summary>
        /// <param name="showInfo">The model containing the choice options.</param>
        /// <returns>The <see cref="Guid"/> of the next node corresponding to the selected option.</returns>
        /// <param name="hideReplicaView"></param>
        public UniTask<Guid> ShowChoice(IChoiceShowInfoProvider showInfo, bool hideReplicaView = false);

        public UniTask WaitForSkip();
    }
}