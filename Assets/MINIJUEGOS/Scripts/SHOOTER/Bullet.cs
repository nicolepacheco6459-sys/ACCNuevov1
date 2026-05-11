using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    public float lifeTime = 2f;

    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
