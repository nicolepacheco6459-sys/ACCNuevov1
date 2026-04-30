using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueEvents : MonoBehaviour
{
    public string minigameSceneName;

    public void LoadMinigame()
    {
        SceneManager.LoadScene(minigameSceneName);
    }
}
