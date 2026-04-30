using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public class ChoiceOptionData : ICloneable
    {
        [field: SerializeField] public SerializableGuid RelatedNodeId { get; set; }
        [field: SerializeField] public string Text { get; set; }

        public ChoiceOptionData() { }

        public ChoiceOptionData(Guid relatedNodeId, string text)
        {
            RelatedNodeId = relatedNodeId;
            Text = text;
        }

        public object Clone()
        {
            var data = new ChoiceOptionData
            {
                RelatedNodeId = RelatedNodeId,
                Text = Text
            };

            return data;
        }
    }
}