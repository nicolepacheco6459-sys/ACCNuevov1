using UnityEngine;

public class NPCDialogueController : MonoBehaviour
{
    public string characterID;

    [Header("Dialogues")]
    public ScriptableObject goodEnding;
    public ScriptableObject neutralEnding;
    public ScriptableObject badEnding;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCharacterDialogue();
        }
    }

    void StartCharacterDialogue()
    {
        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        if (affinity >= 50)
        {
            StartDialogue(goodEnding);
        }
        else if (affinity >= 20)
        {
            StartDialogue(neutralEnding);
        }
        else
        {
            StartDialogue(badEnding);
        }
    }

    void StartDialogue(ScriptableObject dialogue)
    {
        // IMPORTANTE: aquí llamas a tu sistema KMA
        // Ejemplo (puede variar según el asset):
        Object.FindFirstObjectByType<MonoBehaviour>().SendMessage("StartDialogue", dialogue);
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
