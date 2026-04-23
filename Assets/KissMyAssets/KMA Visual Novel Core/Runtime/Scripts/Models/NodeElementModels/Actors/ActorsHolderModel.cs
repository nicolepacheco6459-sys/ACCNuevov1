using System.Collections.Generic;
using System.Linq;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ActorsHolderModel
    {
        public IReadOnlyDictionary<EActorAlignmentType, DialogueActorModel> ActorAlignmentMap { get; }

        public ActorsHolderModel(ActorsHolderData data)
        {
            ActorAlignmentMap = data.Actors.ToDictionary(data => data.Alignment, data => new DialogueActorModel(data));
        }
    }
}

