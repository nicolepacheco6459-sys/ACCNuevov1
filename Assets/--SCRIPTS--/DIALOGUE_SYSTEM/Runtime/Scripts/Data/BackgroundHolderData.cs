using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class BackgroundHolderData
    {
        [field: SerializeField] public SerializableGuid BackgroundId { get; set; }
    }
}
