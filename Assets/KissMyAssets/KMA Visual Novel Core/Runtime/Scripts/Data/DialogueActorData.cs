using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class DialogueActorData : ICloneable
    {
        [field: SerializeField] public SerializableGuid ActorDataId { get; set; }
        [field: SerializeField] public EActorAlignmentType Alignment { get; set; }
        [field: SerializeField] public EActorEmotionType ActorEmotion { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
