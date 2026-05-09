using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_MATCH3 : MonoBehaviour
{
    public static GameManager_MATCH3 Instance;

    [Header("Board")]
    public BoardManager boardManager;

    [Header("UI")]
    public GameObject startPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;

    public TMP_Text scoreText;
    public TMP_Text goalText;
    public TMP_Text levelText;

    [Header("Levels")]
    public LevelData[] levels;

    private int currentLevel = 0;

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startPanel.SetActive(true);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    // =========================================
    // START GAME
    // =========================================

    public void StartGame()
    {
        startPanel.SetActive(false);

        LoadLevel(currentLevel);
    }

    // =========================================
    // LOAD LEVEL
    // =========================================

    void LoadLevel(int levelIndex)
    {
        currentScore = 0;

        LevelData level = levels[levelIndex];

        boardManager.SetupBoard(level);

        UpdateUI();
    }

    // =========================================
    // SCORE
    // =========================================

    public void AddScore(int amount)
    {
        currentScore += amount;

        UpdateUI();

        if (currentScore >= levels[currentLevel].targetScore)
        {
            LevelComplete();
        }
    }

    void UpdateUI()
    {
        scoreText.text =
            currentScore + "/" +
            levels[currentLevel].targetScore;

        goalText.text =
            "Meta: " +
            levels[currentLevel].targetScore;

        levelText.text =
            "Nivel " +
            (currentLevel + 1);
    }

    // =========================================
    // LEVEL COMPLETE
    // =========================================

    void LevelComplete()
    {
        currentLevel++;

        // Si ya terminó todos los niveles
        if (currentLevel >= levels.Length)
        {
            ReturnToMainScene();
        }
        else
        {
            // Regresa al juego principal
            // para continuar diálogo y desbloquear
            // siguiente nivel del minijuego

            ReturnToMainScene();
        }
    }

    // =========================================
    // RETRY
    // =========================================

    public void RetryLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // =========================================
    // RETURN
    // =========================================

    public void ReturnToMainScene()
    {
        SceneManager.LoadScene("Samantha");
    }
}
