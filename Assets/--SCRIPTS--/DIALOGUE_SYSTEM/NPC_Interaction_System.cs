using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteractionSystem : MonoBehaviour
{
    public string characterID;

    [Header("Dialogues por etapa")]
    public ScriptableObject dialogueStage1;
    public ScriptableObject dialogueStage2;
    public ScriptableObject dialogueStage3;

    [Header("Finales")]
    public ScriptableObject goodEnding;
    public ScriptableObject neutralEnding;
    public ScriptableObject badEnding;

    [Header("Minijuego")]
    public string minigameSceneName;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
    }

    void HandleInteraction()
    {
        int stage = CharacterProgress.Instance.GetProgress(characterID);

        if (stage == 0)
        {
            StartDialogue(dialogueStage1);
        }
        else if (stage == 1)
        {
            StartDialogue(dialogueStage2);
        }
        else if (stage == 2)
        {
            StartDialogue(dialogueStage3);
        }
        else
        {
            LaunchEnding();
        }
    }

    void LaunchEnding()
    {
        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        if (affinity >= 50)
            StartDialogue(goodEnding);
        else if (affinity >= 20)
            StartDialogue(neutralEnding);
        else
            StartDialogue(badEnding);
    }

    void StartDialogue(ScriptableObject dialogue)
    {
        if (KMA_DialogueManager.Instance != null)
        {
            KMA_DialogueManager.Instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError("KMA_DialogueManager no encontrado en la escena");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
