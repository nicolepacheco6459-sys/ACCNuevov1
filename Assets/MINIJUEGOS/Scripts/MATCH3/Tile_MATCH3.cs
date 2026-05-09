using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public int iconID;

    public BoardManager board;

    private SpriteRenderer spriteRenderer;

    [Header("Selection")]
    public GameObject selectionObject;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        SetSelected(false);
    }

    // =====================================
    // SETUP
    // =====================================

    public void Setup(
        int newX,
        int newY,
        int newID,
        Sprite sprite,
        BoardManager newBoard)
    {
        x = newX;
        y = newY;

        iconID = newID;

        // IMPORTANTE
        board = newBoard;

        spriteRenderer.sprite = sprite;
    }

    // =====================================
    // SELECTION
    // =====================================

    public void SetSelected(bool selected)
    {
        if (selectionObject != null)
        {
            selectionObject.SetActive(selected);
        }
    }

    // =====================================
    // CLICK
    // =====================================

    private void OnMouseDown()
    {
        // DEBUG
        if (board == null)
        {
            Debug.LogError(
                "BOARD NULL EN TILE"
            );

            return;
        }

        board.SelectTile(this);
    }
}
