using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable] public class ActorGuidInfo : BaseGuidInfo<ActorInfo> { }

    [CreateAssetMenu(fileName = VisualNovelConstants.ActorsConfigName, menuName = VisualNovelConstants.AssetPath + "ActorsConfig", order = 1)]
    public class ActorsInfoConfig : BaseGuidInfoConfig<ActorGuidInfo, ActorInfo>
    {
        private static ActorsInfoConfig _instance;

        public static ActorsInfoConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();

                return _instance;
            }
        }

        private static ActorsInfoConfig Load()
        {
            return Resources.Load<ActorsInfoConfig>(VisualNovelConstants.ActorsConfigName);
        }
    }
}
