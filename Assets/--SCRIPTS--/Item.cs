using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Data")]
    public int ID;

    [SerializeField] private GameObject inventoryPrefab;

    public GameObject GetInventoryPrefab()
    {
        return inventoryPrefab;
    }
}