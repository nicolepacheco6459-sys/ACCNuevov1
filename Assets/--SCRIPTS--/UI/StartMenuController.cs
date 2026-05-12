using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{

    public void OnStartClick()
    {
        SceneManager.LoadScene("PLAYGROUND_Nicole"); // Esta escena tiene que ser cambiada a production una vez finalizado el juego
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


}
