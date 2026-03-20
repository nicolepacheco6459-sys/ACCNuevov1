using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void SetItem(GameObject item)
    {
        currentItem = item;
    }

    public void ClearSlot()
    {
        currentItem = null;
    }
}