using UnityEngine;

public class KMA_DialogueManager : MonoBehaviour
{
    public static KMA_DialogueManager Instance;

    public MonoBehaviour dialogueWindow; // aquí irá NovelSampleDialogueWindow

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(ScriptableObject dialogue)
    {
        dialogueWindow.SendMessage("StartDialogue", dialogue, SendMessageOptions.DontRequireReceiver);
    }
}
