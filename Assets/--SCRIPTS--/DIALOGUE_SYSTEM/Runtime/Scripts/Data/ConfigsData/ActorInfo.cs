using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class ActorInfo
    {
        public string Title;
        public string Name;
        public List<EmotionSpritePair> Sprites;

        public IReadOnlyDictionary<EActorEmotionType, Sprite> SpritesMap => Sprites.ToDictionary(pair => pair.Emotion, pair => pair.Sprite);
    }
}
