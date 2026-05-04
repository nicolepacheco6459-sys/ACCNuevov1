using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager_CARDS : MonoBehaviour
{
    public static UIManager_CARDS instance;

    public GameObject startPanel;
    public GameObject losePanel;

    public TMP_Text difficultyText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startPanel.SetActive(true);
        losePanel.SetActive(false);

        Time.timeScale = 0f;

        UpdateDifficultyText();
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        Time.timeScale = 1f;

        
        CardManager cardManager = Object.FindFirstObjectByType<CardManager>();

        if (cardManager != null)
        {
            // Llama a CreatePlayField en vez de GenerateBoard
            cardManager.SendMessage("CreatePlayField");
        }
        else
        {
            Debug.LogError("No se encontró CardManager en la escena");
        }
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [SerializeField] string mainSceneName = "Samantha";
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        // ⚠️ Cambia esto por tu escena real
        SceneManager.LoadScene(mainSceneName);
    }

    void UpdateDifficultyText()
    {
        string difficulty = "";

        switch (DifficultyManager.instance.currentLevel)
        {
            case 0:
                difficulty = "Fácil";
                break;

            case 1:
                difficulty = "Medio";
                break;

            case 2:
                difficulty = "Difícil";
                break;
        }

        difficultyText.text = "Dificultad: " + difficulty;
    }
}
