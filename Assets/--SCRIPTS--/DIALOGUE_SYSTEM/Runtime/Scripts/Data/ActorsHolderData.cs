using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class ActorsHolderData : ICloneable
    {
        [field: SerializeReference] public List<DialogueActorData> Actors { get; set; } = new List<DialogueActorData>();

        public object Clone()
        {
            var data = new ActorsHolderData
            {
                Actors = Actors.Select(x => x.Clone() as DialogueActorData).ToList()
            };

            return data;
        }
    }
}