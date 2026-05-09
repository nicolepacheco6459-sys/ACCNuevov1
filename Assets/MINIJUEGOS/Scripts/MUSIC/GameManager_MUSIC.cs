using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_MUSIC : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject gameUI;
    public GameObject gameOverMenu;

    public SongManager songManager;

    bool gameStarted = false;

    void Start()
    {
        Time.timeScale = 0f;
        startMenu.SetActive(true);
        gameUI.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startMenu.SetActive(false);
        gameUI.SetActive(true);
        gameStarted = true;

        songManager.StartSong();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverMenu.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Samantha"); 
    }

    public GameObject winMenu;

    public void Win()
    {
        Time.timeScale = 0f;
        winMenu.SetActive(true);
    }
}
