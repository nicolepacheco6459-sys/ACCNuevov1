using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float spawnRange = 8f;

    [Header("Player Reference")]
    public Transform player;

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector2 spawnPosition = (Vector2)player.position + (randomDirection * spawnRange);

        return spawnPosition;
    }
}