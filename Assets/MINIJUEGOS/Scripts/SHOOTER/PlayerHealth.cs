using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public HeartUI heartUI;

    public GameObject gameOverPanel;

    public void SetupHealth()
    {
        maxHealth = DifficultySettings.startHealth;

        currentHealth = maxHealth;

        heartUI.UpdateHearts(currentHealth);
    }

    public void TakeDamage()
    {
        currentHealth--;

        heartUI.UpdateHearts(currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void Heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            heartUI.UpdateHearts(currentHealth);
        }
    }

    void GameOver()
    {
        Time.timeScale = 0f;

        GameManager_SHOOTER.instance.GameOver();
    }
}