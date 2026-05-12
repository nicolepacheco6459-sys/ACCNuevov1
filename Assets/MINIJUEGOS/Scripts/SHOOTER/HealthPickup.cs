using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TOCANDO ALGO");

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER TOCO VIDA");

            PlayerHealth playerHealth =
                other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal();

                Destroy(gameObject);
            }
        }
    }
}
