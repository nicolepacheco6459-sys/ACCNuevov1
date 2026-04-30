using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundary;
    [SerializeField] private Direction direction;

    private CinemachineConfiner2D confiner;
    private CinemachineCamera _mainCamera;

    private enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner = Object.FindFirstObjectByType<CinemachineConfiner2D>();
        
        // Buscar la cámara por nombre
        GameObject camObj = GameObject.Find("CmCam");
        if (camObj != null)
            _mainCamera = camObj.GetComponent<CinemachineCamera>();
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
            UpdateCameraPosition(collision.gameObject);

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

    private void UpdateCameraPosition(GameObject player)
    {
        if (_mainCamera != null)
        {
            _mainCamera.transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                _mainCamera.transform.position.z
            );
        }
    }
}