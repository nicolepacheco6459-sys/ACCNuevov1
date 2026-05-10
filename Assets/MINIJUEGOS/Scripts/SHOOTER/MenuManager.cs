using DG.Tweening.Core.Easing;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject hudPanel;

    public PlayerHealth playerHealth;
    public GameManager_SHOOTER gameManager;

    public void EasyMode()
    {
        StartGame(5);
    }

    public void MediumMode()
    {
        StartGame(4);
    }

    public void HardMode()
    {
        StartGame(3);
    }

    void StartGame(int health)
    {
        DifficultySettings.startHealth = health;

        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);

        playerHealth.SetupHealth();
        gameManager_.StartGame();
    }
}
