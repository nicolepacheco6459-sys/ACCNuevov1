using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ActorViewData
    {
        public string Name { get; }
        public Sprite Sprite { get; }

        public ActorViewData(string name, Sprite sprite)
        {
            Name = name;
            Sprite = sprite;
        }
    }
}
