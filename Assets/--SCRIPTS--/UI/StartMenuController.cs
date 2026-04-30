using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("--SCENES--/Escena Principal v2");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}
