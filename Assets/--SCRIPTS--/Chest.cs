using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private bool isOpened;
    public bool IsOpened { get { return isOpened; } private set { isOpened = value; } }
    public string ChestID; // Identificador único para el cofre
    public GameObject itemPrefab;
    public Sprite openSprite;

    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);

    }
    public bool CanInteract()
    {
        return !IsOpened; 
    }
    public void Interact()
    {
        if (!CanInteract()) return;
        //Cofre abierto
        OpenChest();
    }

    private void OpenChest()
    {
        SetOpened(true);

        //Tirar Item
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            var bounceEffect = droppedItem.GetComponent<BounceEffect>();
            if (bounceEffect != null)
                bounceEffect.StartBounce();
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;

        if (opened)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null && openSprite != null)
                sr.sprite = openSprite;
        }
    }
}
