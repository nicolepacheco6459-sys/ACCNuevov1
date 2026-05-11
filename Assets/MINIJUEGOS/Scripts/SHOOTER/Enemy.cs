using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    public int health = 1;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Destroy(gameObject, 20f);
    }

    void Update()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();

            Destroy(gameObject);
        }
    }
}
