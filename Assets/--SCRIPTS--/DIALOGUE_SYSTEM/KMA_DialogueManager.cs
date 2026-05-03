using UnityEngine;
using KissMyAssets.VisualNovelCore.Runtime;
using Cysharp.Threading.Tasks;

public class KMA_DialogueManager : MonoBehaviour
{
    public static KMA_DialogueManager Instance;

    public NovelSampleDialogueWindow dialogueWindow;

    public bool IsPlaying { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    public async UniTask StartDialogueAsync()
    {
        if (dialogueWindow == null)
        {
            Debug.LogError("DialogueWindow NO asignado en KMA_DialogueManager");
            return;
        }

        Debug.Log("▶ Iniciando diálogo KMA");

        IsPlaying = true;

        await dialogueWindow.PlayDialogues();

        IsPlaying = false;

        Debug.Log("✔ Diálogo terminado");
    }

    // Mantener compatibilidad con tu sistema actual
    public void StartDialogue()
    {
        StartDialogueAsync().Forget();
    }
}