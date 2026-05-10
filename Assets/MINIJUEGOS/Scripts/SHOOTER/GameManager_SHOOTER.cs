using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_SHOOTER : MonoBehaviour
{
    public EnemySpawner spawner;
    public HealthSpawner healthSpawner;

    public int currentLevel = 1;

    public float levelDuration = 60f;
    private float timer;

    private bool gameStarted = false;

    public void StartGame()
    {
        gameStarted = true;

        currentLevel = 1;
        timer = levelDuration;

        SetupLevel();
    }

    void Update()
    {
        if (!gameStarted) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            NextLevel();
        }
    }

    void SetupLevel()
    {
        switch (currentLevel)
        {
            case 1:
                spawner.spawnRate = 2f;
                healthSpawner.spawnRate = 10f;
                break;

            case 2:
                spawner.spawnRate = 1.3f;
                healthSpawner.spawnRate = 15f;
                break;

            case 3:
                spawner.spawnRate = 0.8f;
                healthSpawner.spawnRate = 25f;
                break;
        }
    }

    void NextLevel()
    {
        currentLevel++;

        if (currentLevel > 3)
        {
            YouWin();
            return;
        }

        timer = levelDuration;

        SetupLevel();
    }

    void YouWin()
    {
        Debug.Log("YOU WIN");

        SceneManager.LoadScene("Samantha");
    }
}