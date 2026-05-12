using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_SHOOTER : MonoBehaviour
{
    public static GameManager_SHOOTER instance;
    public PlayerHealth playerHealth;

    [Header("Spawners")]
    public EnemySpawner spawner;
    public HealthSpawner healthSpawner;

    [Header("Panels")]
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    [Header("Level Settings")]
    public int currentLevel;

    public float levelDuration = 60f;

    private float timer;

    private bool gameStarted = false;

    public float GetRemainingTime()
    {
        return timer;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        spawner.enabled = false;
        healthSpawner.enabled = false;
    }

    public void StartGame()
    {
        gameStarted = true;

        currentLevel = ShooterProgress.currentLevel;

        timer = levelDuration;

        hudPanel.SetActive(true);

        spawner.enabled = true;
        healthSpawner.enabled = true;

        playerHealth.SetupHealth();

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
        ShooterProgress.currentLevel++;

        SceneManager.LoadScene("Samantha");
    }

    public void GameOver()
    {
        gameStarted = false;

        spawner.enabled = false;
        healthSpawner.enabled = false;

        Time.timeScale = 0f;

        gameOverPanel.SetActive(true);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Samantha");
    }
}