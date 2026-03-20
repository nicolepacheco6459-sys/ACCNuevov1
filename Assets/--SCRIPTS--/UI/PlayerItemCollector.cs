using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();

            if (item != null)
            {
                GameObject prefab = item.GetInventoryPrefab();

                bool added = inventoryController.AddItem(prefab);

                if (added)
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}