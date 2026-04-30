using UnityEngine;
using KissMyAssets.VisualNovelCore.Runtime;
using System.Collections.Generic;

public class NPCInteractionSystem : MonoBehaviour, IInteractable
{
    [Header("ID del personaje")]
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
    // INTERFACE
    // =========================

    public bool CanInteract()
    {
        return !isInteracting;
    }

    public void Interact()
    {
        if (isInteracting) return;

        isInteracting = true;
        HandleInteraction();
    }

    // =========================
    // LÓGICA PRINCIPAL
    // =========================

    void HandleInteraction()
    {
        // 🔥 VALIDACIÓN IMPORTANTE
        if (string.IsNullOrEmpty(characterID))
        {
            Debug.LogError("NPC sin characterID asignado");
            return;
        }

        // 🔥 ASIGNAR PERSONAJE ACTUAL PARA AFINIDAD
        if (AffinityChoiceHandler.Instance != null)
        {
            AffinityChoiceHandler.Instance.currentCharacterID = characterID;
        }

        // 🔥 ACTUALIZAR UI DE AFINIDAD
        if (AffinityUI.Instance != null)
        {
            AffinityUI.Instance.SetCharacter(characterID);
        }

        // 🔥 OBTENER PROGRESO
        int stage = 0;

        if (CharacterProgress.Instance != null)
        {
            stage = CharacterProgress.Instance.GetProgress(characterID);
        }
        else
        {
            Debug.LogWarning("CharacterProgress no encontrado");
        }

        // 🔥 SELECCIÓN DE DIÁLOGO
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
        if (AffinitySystem.Instance == null)
        {
            Debug.LogError("AffinitySystem no encontrado");
            return;
        }

        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        if (affinity >= 50)
            PlayDialogue(goodEnding);
        else if (affinity >= 20)
            PlayDialogue(neutralEnding);
        else
            PlayDialogue(badEnding);
    }

    // =========================
    // EJECUTAR DIÁLOGO
    // =========================

    void PlayDialogue(DialogueSceneConfig dialogue)
    {
        if (KMA_DialogueManager.Instance == null)
        {
            Debug.LogError("KMA_DialogueManager no encontrado");
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

        // 🔥 CAMBIAR DIÁLOGO DINÁMICAMENTE
        window.GetType()
            .GetField("_dialogueScenes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(window, new List<DialogueSceneConfig> { dialogue });

        // 🔥 INICIAR DIÁLOGO
        KMA_DialogueManager.Instance.StartDialogue();

        // 🔥 LIBERAR INTERACCIÓN DESPUÉS DE UN MOMENTO
        Invoke(nameof(ResetInteraction), 1.5f);
    }

    void ResetInteraction()
    {
        isInteracting = false;
    }
}


