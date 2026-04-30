using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class DialogueActorModel
    {
        public DialogueTextModel Name { get; }
        public Sprite Sprite { get; }

        public DialogueActorModel(DialogueActorData data)
        {
            var info = ActorsInfoConfig.Instance.GetData(data.ActorDataId);

            if (info == null)
            {
                Debug.LogWarning($"DialogueActorModel: ActorInfo with ID {data.ActorDataId} not found in ActorsInfoConfig.");
                return;
            }

            Name = new DialogueTextModel(info.Name);
            Sprite = info.SpritesMap[data.ActorEmotion];
        }
    }
}
