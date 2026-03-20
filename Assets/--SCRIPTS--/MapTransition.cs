using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundary;
    [SerializeField] private Direction direction;

    private CinemachineConfiner2D confiner;

    private enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner = Object.FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (confiner != null)
            {
                confiner.BoundingShape2D = mapBoundary;
                confiner.InvalidateBoundingShapeCache();
            }

            UpdatePlayerPosition(collision.gameObject);

            MapController_Manual.Instance?.HighlighArea(mapBoundary.name);
            MapController_Dynamic.Instance?.UpdateCurrentArea(mapBoundary.name);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += 1f;
                break;
            case Direction.Down:
                newPos.y -= 1f;
                break;
            case Direction.Left:
                newPos.x -= 1f;
                break;
            case Direction.Right:
                newPos.x += 1f;
                break;
        }

        player.transform.position = newPos;
    }
}