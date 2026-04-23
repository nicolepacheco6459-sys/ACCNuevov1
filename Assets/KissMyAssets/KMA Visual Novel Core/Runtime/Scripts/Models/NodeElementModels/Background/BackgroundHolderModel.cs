using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class BackgroundHolderModel
    {
        public Sprite BackgroundSprite { get; }

        public BackgroundHolderModel(BackgroundHolderData data)
        {
            BackgroundInfo info = BackgroundsInfoConfig.Instance.GetData(data.BackgroundId);

            if(info != null)
                BackgroundSprite = info.Sprite;
        }
    }
}
