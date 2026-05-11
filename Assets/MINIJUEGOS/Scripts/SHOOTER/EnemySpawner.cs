using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject level1Enemy;
    public GameObject level2Enemy;
    public GameObject level3Enemy;

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
        GameObject enemyToSpawn = GetEnemyForCurrentLevel();

        if (enemyToSpawn == null)
        {
            Debug.LogError("NO HAY ENEMIGO ASIGNADO");

            return;
        }

        Vector2 spawnPosition = GetRandomSpawnPosition();

        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

        Debug.Log("ENEMY SPAWNED");
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector2 spawnPosition =
            (Vector2)player.position + (randomDirection * spawnRange);

        return spawnPosition;
    }

    GameObject GetEnemyForCurrentLevel()
    {
        switch (GameManager_SHOOTER.instance.currentLevel)
        {
            case 1:
                return level1Enemy;

            case 2:
                return level2Enemy;

            case 3:
                return level3Enemy;

            default:
                return level1Enemy;
        }
    }
}