using UnityEngine;
using KissMyAssets.VisualNovelCore.Runtime;
using System.Collections.Generic;

public class NPCInteractionSystem : MonoBehaviour, IInteractable
{
    public string characterID;

    [Header("Dialogues por etapa")]
    public DialogueSceneConfig dialogueStage1;
    public DialogueSceneConfig dialogueStage2;
    public DialogueSceneConfig dialogueStage3;

    [Header("Finales")]
    public DialogueSceneConfig goodEnding;
    public DialogueSceneConfig neutralEnding;
    public DialogueSceneConfig badEnding;

    private bool isInteracting = false;

    // =========================
    // INTERFACE (InteractionDetector usa esto)
    // =========================

    public bool CanInteract()
    {
        return !isInteracting;
    }

    public void Interact()
    {
        HandleInteraction();
    }

    // =========================
    // LÓGICA PRINCIPAL
    // =========================

    void HandleInteraction()
    {
        if (CharacterProgress.Instance == null || AffinitySystem.Instance == null)
        {
            Debug.LogError("Falta CharacterProgress o AffinitySystem en la escena");
            return;
        }

        isInteracting = true;

        int stage = CharacterProgress.Instance.GetProgress(characterID);

        if (stage == 0)
        {
            PlayDialogue(dialogueStage1);
        }
        else if (stage == 1)
        {
            PlayDialogue(dialogueStage2);
        }
        else if (stage == 2)
        {
            PlayDialogue(dialogueStage3);
        }
        else
        {
            LaunchEnding();
        }
    }

    // =========================
    // FINALES
    // =========================

    void LaunchEnding()
    {
        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        if (affinity >= 50)
            PlayDialogue(goodEnding);
        else if (affinity >= 20)
            PlayDialogue(neutralEnding);
        else
            PlayDialogue(badEnding);
    }

    // =========================
    // EJECUTAR DIÁLOGO KMA
    // =========================

    void PlayDialogue(DialogueSceneConfig dialogue)
    {
        if (KMA_DialogueManager.Instance == null)
        {
            Debug.LogError("KMA_DialogueManager no encontrado en la escena");
            return;
        }

        var window = KMA_DialogueManager.Instance.dialogueWindow as NovelSampleDialogueWindow;

        if (window == null)
        {
            Debug.LogError("DialogueWindow no es NovelSampleDialogueWindow");
            return;
        }

        if (dialogue == null)
        {
            Debug.LogError("DialogueSceneConfig no asignado en NPC");
            return;
        }

        // 🔥 Cambia el diálogo dinámicamente
        window.GetType()
            .GetField("_dialogueScenes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(window, new List<DialogueSceneConfig> { dialogue });

        // 🔥 Ejecuta el diálogo
        KMA_DialogueManager.Instance.StartDialogue();

        // 🔥 Permitir volver a interactuar después (opcional)
        Invoke(nameof(ResetInteraction), 1f);
    }

    void ResetInteraction()
    {
        isInteracting = false;
    }
    public void Interaction()
    {
        Debug.Log("Interactuando con NPC nuevo sistema");
        HandleInteraction();
    }
}


