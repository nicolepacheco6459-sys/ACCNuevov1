using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject hudPanel;

    public PlayerHealth playerHealth;
    public GameManager_SHOOTER gameManager;

    public void StartButton()
    {
        mainMenuPanel.SetActive(false);

        hudPanel.SetActive(true);

        playerHealth.SetupHealth();

        gameManager.StartGame();
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Samantha");
    }
}