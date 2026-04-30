using UnityEngine;
using KissMyAssets.VisualNovelCore.Runtime;

public class KMA_DialogueManager : MonoBehaviour
{
    public static KMA_DialogueManager Instance;

    public NovelSampleDialogueWindow dialogueWindow;

    private void Awake()
    {
        Instance = this;
    }

    public async void StartDialogue()
    {
        if (dialogueWindow == null)
        {
            Debug.LogError("DialogueWindow NO asignado en KMA_DialogueManager");
            return;
        }

        Debug.Log("▶ Iniciando diálogo KMA");

        await dialogueWindow.PlayDialogues();
    }
}