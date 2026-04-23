using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class EmotionSpritePair
    {
        [field: SerializeField] public EActorEmotionType Emotion { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }

        public EmotionSpritePair(EActorEmotionType emotion)
        {
            Emotion = emotion;
        }
    }
}