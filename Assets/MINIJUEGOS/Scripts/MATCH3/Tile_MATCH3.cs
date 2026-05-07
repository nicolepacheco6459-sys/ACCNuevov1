using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public int iconID;

    public BoardManager board;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

        board = newBoard;

        spriteRenderer.sprite = sprite;
    }

    private void OnMouseDown()
    {
        board.SelectTile(this);
    }
}
