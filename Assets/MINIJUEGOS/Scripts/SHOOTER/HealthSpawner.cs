using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    [Header("Health Pickup")]
    public GameObject healthPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 10f;
    public float spawnRange = 7f;

    [Header("Player Reference")]
    public Transform player;

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnHealth();

            spawnTimer = 0f;
        }
    }

    void SpawnHealth()
    {
        Vector2 spawnPosition = GetRandomPosition();

        Instantiate(healthPrefab, spawnPosition, Quaternion.identity);
    }

    Vector2 GetRandomPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector2 randomPosition = (Vector2)player.position + (randomDirection * spawnRange);

        return randomPosition;
    }
}
