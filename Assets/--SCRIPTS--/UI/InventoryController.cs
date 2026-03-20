using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class InventoryController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ItemDictionary itemDictionary;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;

    [Header("Configuración")]
    [SerializeField] private int slotCount = 10;

    [Header("Items Iniciales (Opcional)")]
    [SerializeField] private GameObject[] initialItemPrefabs;

    private bool slotsCreated = false;

    void Awake()
    {
        if (itemDictionary == null)
            itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    void Start()
    {
        CreateEmptySlots();
        PlaceInitialItems(); // <- vuelve a activarse
    }

    void CreateEmptySlots()
    {
        if (slotsCreated) return;

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        slotsCreated = true;
    }

    // ==========================
    // COLOCAR ITEMS INICIALES
    // ==========================

    void PlaceInitialItems()
    {
        if (initialItemPrefabs == null) return;

        for (int i = 0; i < initialItemPrefabs.Length; i++)
        {
            if (i >= inventoryPanel.transform.childCount)
                break;

            Slot slot = inventoryPanel.transform
                .GetChild(i)
                .GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(initialItemPrefabs[i], slot.transform);

                RectTransform rect = newItem.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;

                slot.currentItem = newItem;
            }
        }
    }

    // ==========================
    // AGREGAR ITEM DINÁMICAMENTE
    // ==========================

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);

                RectTransform rect = newItem.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;

                slot.currentItem = newItem;

                return true;
            }
        }

        Debug.Log("Inventario lleno");
        return false;
    }

    // ==========================
    // GUARDAR
    // ==========================

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        foreach (Transform slotTransform in inventoryPanel.transform)  //Checar esta funcion para ver que parte está fallando de línea a línea el código (Si los trae es al momento de guardarlo, sino hay que averiguar por que no accede al item)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null)
                {
                    invData.Add(new InventorySaveData
                    {
                        itemID = item.ID,
                        slotIndex = slotTransform.GetSiblingIndex()
                    });
                }
            }
        }

        return invData;
    }

    [System.Serializable]
    private class InventorySaveDataArray
    {
        public InventorySaveData[] items;
    }

    public string GetInventoryJson()
    {
        List<InventorySaveData> items = GetInventoryItems();

        if (items == null || items.Count == 0)
            return "[]";

        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        for (int i = 0; i < items.Count; i++)
        {
            sb.Append(JsonUtility.ToJson(items[i]));
            if (i < items.Count - 1) sb.Append(",");
        }

        sb.Append("]");
        return sb.ToString();
    }

    public void SetInventoryFromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        string trimmed = json.Trim();

        if (trimmed == "[]")
        {
            SetInventoryItems(new List<InventorySaveData>());
            return;
        }

        string wrapped = "{\"items\":" + json + "}";
        InventorySaveDataArray arr = JsonUtility.FromJson<InventorySaveDataArray>(wrapped);

        List<InventorySaveData> list = new List<InventorySaveData>();
        if (arr != null && arr.items != null)
            list.AddRange(arr.items);

        SetInventoryItems(list);
    }

    // ==========================
    // CARGAR
    // ==========================

    public void SetInventoryItems(List<InventorySaveData> inventoryData)
    {
        if (inventoryData == null)
            return;

        if (!slotsCreated)
            CreateEmptySlots();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }

        foreach (InventorySaveData data in inventoryData)
        {
            if (data.slotIndex >= inventoryPanel.transform.childCount)
                continue;

            Slot slot = inventoryPanel.transform
                .GetChild(data.slotIndex)
                .GetComponent<Slot>();

            if (slot == null) continue;

            GameObject prefab = itemDictionary.GetItemPrefab(data.itemID);

            if (prefab != null)
            {
                GameObject item = Instantiate(prefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                item.GetComponent<RectTransform>().localScale = Vector3.one;

                slot.currentItem = item;
            }
        }
    }
}