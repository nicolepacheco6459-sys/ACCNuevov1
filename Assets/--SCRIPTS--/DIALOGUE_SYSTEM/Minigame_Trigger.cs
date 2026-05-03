using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    public string minigameID;
    public string sceneName;

    public bool CanInteract()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning("GameProgressManager no encontrado");
            return false;
        }

        return GameProgressManager.Instance.IsUnlocked(minigameID);
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            Debug.Log("Minijuego bloqueado");
            return;
        }

        Debug.Log("Entrando al minijuego: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
