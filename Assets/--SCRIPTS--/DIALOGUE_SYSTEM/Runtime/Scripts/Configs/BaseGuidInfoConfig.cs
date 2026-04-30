using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Base abstract class for data entries that are uniquely identified by a <see cref="Guid"/>.
    /// Used within configuration ScriptableObjects to store runtime data linked to a stable ID.
    /// </summary>
    /// <typeparam name="TData">The actual payload data structure associated with the GUID.</typeparam>
    [Serializable]
    public abstract class BaseGuidInfo<TData>
    {
        /// <summary>
        /// The unique serializable identifier for this entry.
        /// </summary>
        public SerializableGuid Guid;

        /// <summary>
        /// The actual data payload associated with the GUID.
        /// </summary>
        public TData Data;
    }

    /// <summary>
    /// Base abstract class for ScriptableObjects that serve as centralized storage
    /// for GUID-mapped data (e.g., Actors, Backgrounds, Items). 
    /// It provides a runtime-ready dictionary (<see cref="DataMap"/>) for quick lookup by GUID.
    /// </summary>
    /// <typeparam name="TEntry">The specific type of <see cref="BaseGuidInfo{TData}"/> that wraps the data.</typeparam>
    /// <typeparam name="TData">The actual data structure stored inside the entry.</typeparam>
    public abstract class BaseGuidInfoConfig<TEntry, TData> : ScriptableObject
        where TEntry : BaseGuidInfo<TData>, new()
    {
        [SerializeField]
        private List<TEntry> _entries = new List<TEntry>(); // The list serialized by Unity

        // Runtime cache for quick GUID lookup
        private Dictionary<Guid, TData> _dataMap;

        /// <summary>
        /// Gets the raw list of data entries, primarily used by the editor for modification.
        /// </summary>
        public List<TEntry> Entries => _entries;

        /// <summary>
        /// Gets a read-only dictionary mapping all unique GUIDs to their respective data payloads.
        /// The map is lazily initialized and rebuilt if the underlying list is modified.
        /// </summary>
        public IReadOnlyDictionary<Guid, TData> DataMap
        {
            get
            {
                // Initialize/rebuild the map if it's null (e.g., after loading or modification)
                if (_dataMap == null)
                {
                    _dataMap = new Dictionary<Guid, TData>();

                    foreach (var entry in _entries)
                    {
                        // Populating the dictionary with GUID as key
                        _dataMap[entry.Guid] = entry.Data;
                    }
                }

                return _dataMap;
            }
        }

        /// <summary>
        /// Creates a new entry with a fresh GUID and adds it to the configuration.
        /// Invalidates the runtime data map cache.
        /// </summary>
        /// <param name="dataFactory">Optional factory function to create the initial data payload.</param>
        public void AddNewEntry(Func<TData> dataFactory = null)
        {
            // Create a new entry with a unique GUID
            var entry = new TEntry { Guid = Guid.NewGuid(), Data = dataFactory != null ? dataFactory() : default };

            _entries.Add(entry);
            _dataMap = null; // Invalidate cache
        }

        /// <summary>
        /// Retrieves the data payload associated with the given GUID.
        /// </summary>
        /// <param name="id">The GUID of the desired data entry.</param>
        /// <returns>The data payload, or the default value of TData if not found.</returns>
        public TData GetData(Guid id)
        {
            if (DataMap.TryGetValue(id, out var data))
            {
                return data;
            }

            // Return default if the GUID is not found
            return default;
        }
    }
}