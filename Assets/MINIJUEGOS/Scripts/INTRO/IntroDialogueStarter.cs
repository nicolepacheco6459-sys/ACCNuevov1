using UnityEngine;
using UnityEngine.SceneManagement;
using KissMyAssets.VisualNovelCore.Runtime;
using System.Collections;
using System.Collections.Generic;

public class IntroDialogueStarter : MonoBehaviour
{
    [Header("Dialogos de Intro")]
    public DialogueSceneConfig intro1;
    public DialogueSceneConfig intro2;
    public DialogueSceneConfig intro3;
    public DialogueSceneConfig intro4;
    public DialogueSceneConfig intro5;

    private void Start()
    {
        StartIntro();
    }

    void StartIntro()
    {
        if (KMA_DialogueManager.Instance == null)
        {
            Debug.LogError("KMA Dialogue Manager no encontrado");
            return;
        }

        var window = KMA_DialogueManager.Instance.dialogueWindow as NovelSampleDialogueWindow;

        if (window == null)
        {
            Debug.LogError("DialogueWindow incorrecto");
            return;
        }

        //  Lista de diálogos
        List<DialogueSceneConfig> introDialogues = new List<DialogueSceneConfig>();

        if (intro1 != null) introDialogues.Add(intro1);
        if (intro2 != null) introDialogues.Add(intro2);
        if (intro3 != null) introDialogues.Add(intro3);
        if (intro4 != null) introDialogues.Add(intro4);
        if (intro5 != null) introDialogues.Add(intro5);

        //  Asignar diálogos
        window.GetType()
            .GetField("_dialogueScenes",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            .SetValue(window, introDialogues);

        //  Iniciar diálogo
        KMA_DialogueManager.Instance.StartDialogue();

        Debug.Log("Intro iniciada");

        //  Esperar a que termine TODO
        StartCoroutine(WaitForIntroEnd());
    }

    IEnumerator WaitForIntroEnd()
    {
        // Esperar un frame para evitar falso positivo
        yield return new WaitForSeconds(0.2f);

        //  Esperar hasta que el dialogue window se cierre
        while (KMA_DialogueManager.Instance != null &&
               KMA_DialogueManager.Instance.dialogueWindow.gameObject.activeSelf)
        {
            yield return null;
        }

        Debug.Log("Intro terminada");

        //  Cargar siguiente escena
        SceneManager.LoadScene("Choose Character");
    }
}
