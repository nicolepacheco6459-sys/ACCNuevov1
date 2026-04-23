using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [Serializable]
    public struct SerializableGuid
    {
        [SerializeField] private string _value;

        public Guid Value
        {
            readonly get
            {
                return string.IsNullOrEmpty(_value) ? Guid.Empty : new Guid(_value);
            }

            set
            {
                _value = value.ToString();
            }
        }

        public override readonly string ToString()
        {
            return _value;
        }

        public static implicit operator Guid(SerializableGuid s) => s.Value;

        public static implicit operator SerializableGuid(Guid g) => new() { Value = g };
    }
}