using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    public string minigameID;
    public string sceneName;

    private bool playerInRange = false;

    public bool CanInteract()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError("GameProgressManager no encontrado");
            return false;
        }

        bool unlocked = GameProgressManager.Instance.IsUnlocked(minigameID);

        Debug.Log("Minijuego " + minigameID + " desbloqueado: " + unlocked);

        return unlocked;
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            Debug.Log("Minijuego bloqueado");
            return;
        }

        Debug.Log("🎮 Entrando al minijuego: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Jugador en zona de minijuego");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
