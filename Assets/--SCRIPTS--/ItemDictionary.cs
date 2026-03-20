using UnityEngine;
using System.Collections.Generic;

public class ItemDictionary : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemPrefabs;

    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        foreach (GameObject prefab in itemPrefabs)
        {
            if (prefab == null)
                continue;

            Item item = prefab.GetComponent<Item>();
            if (item == null)
            {
                Debug.LogWarning($"Prefab {prefab.name} does not have an Item component.");
                continue;
            }

            if (item.ID <= 0)
            {
                Debug.LogWarning($"Item {item.name} has an invalid ID ({item.ID}). IDs must be positive and unique.");
                continue;
            }

            if (itemDictionary.ContainsKey(item.ID))
            {
                Debug.LogWarning($"Duplicate Item ID detected: {item.ID} (prefab {prefab.name}).");
            }
            else
            {
                itemDictionary.Add(item.ID, prefab);
            }
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        if (itemDictionary.TryGetValue(itemID, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"Item with ID {itemID} not found in dictionary");
        return null;
    }
}