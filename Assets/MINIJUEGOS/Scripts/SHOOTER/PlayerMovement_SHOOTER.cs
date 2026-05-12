using UnityEngine;

public class PlayerMovement_SHOOTER : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;

    private Vector2 movement;

    [Header("Screen Limits")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * speed;

        Vector2 clampedPosition = rb.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        rb.position = clampedPosition;
    }
}
