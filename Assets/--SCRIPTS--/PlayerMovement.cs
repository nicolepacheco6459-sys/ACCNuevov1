using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; //Variable de velocidad

    private Rigidbody2D rb2D; 
    private Vector2 movementInput; //Variable de movimiento
    private Animator animator;  //Animator controler del player

    void Start ()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        // read input every frame unless the game is paused
        if (PauseController.IsGamePaused)
        {
            movementInput = Vector2.zero;
            rb2D.linearVelocity = Vector2.zero; // Detener movimiento inmediato
            animator.SetFloat("Speed", 0);
            return;
        }

        Move();
    }

    void Move()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput = movementInput.normalized;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);
    }

    void FixedUpdate()
    {
        rb2D.linearVelocity = movementInput * speed;
    }
    
}