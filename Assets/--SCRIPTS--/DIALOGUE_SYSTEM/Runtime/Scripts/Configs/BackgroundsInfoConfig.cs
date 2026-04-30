using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable] public class BackgroundGuidInfo : BaseGuidInfo<BackgroundInfo> { }

    [CreateAssetMenu(fileName = VisualNovelConstants.BackgroundsConfigName, menuName = VisualNovelConstants.AssetPath + "BackgroundsConfig", order = 2)]
    public class BackgroundsInfoConfig : BaseGuidInfoConfig<BackgroundGuidInfo, BackgroundInfo>
    {
        private static BackgroundsInfoConfig _instance;

        public static BackgroundsInfoConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();

                return _instance;
            }
        }

        private static BackgroundsInfoConfig Load()
        {
            return Resources.Load<BackgroundsInfoConfig>(VisualNovelConstants.BackgroundsConfigName);
        }
    }
}
