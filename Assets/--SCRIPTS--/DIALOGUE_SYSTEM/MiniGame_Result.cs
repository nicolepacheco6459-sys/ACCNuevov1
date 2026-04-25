using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameResult : MonoBehaviour
{
    public string characterID;

    public void WinGame()
    {
        AffinitySystem.Instance.AddAffinity(characterID, 15);
        CharacterProgress.Instance.AdvanceProgress(characterID);

        SceneManager.LoadScene("MainScene"); // regresa al juego
    }

    public void LoseGame()
    {
        AffinitySystem.Instance.AddAffinity(characterID, 5);
        CharacterProgress.Instance.AdvanceProgress(characterID);

        SceneManager.LoadScene("MainScene");
    }
}
