using UnityEngine;
using KissMyAssets.VisualNovelCore.Runtime;
using System.Collections;
using System.Collections.Generic;

public class NPCInteractionSystem : MonoBehaviour, IInteractable
{
    [Header("ID del personaje")]
    public string characterID;

    [Header("Dialogos principales")]
    public DialogueSceneConfig dialogueStage1;
    public DialogueSceneConfig dialogueStage2;
    public DialogueSceneConfig dialogueStage3;

    [Header("Dialogos de minijuego")]
    public DialogueSceneConfig goToMinigame1;
    public DialogueSceneConfig goToMinigame2;
    public DialogueSceneConfig goToMinigame3;

    [Header("Minijuegos")]
    public string minigame1ID;
    public string minigame2ID;
    public string minigame3ID;

    [Header("Finales")]
    public DialogueSceneConfig goodEnding;
    public DialogueSceneConfig neutralEnding;
    public DialogueSceneConfig badEnding;

    private bool isInteracting = false;

    public bool CanInteract() => !isInteracting;

    public void Interact()
    {
        if (isInteracting) return;

        isInteracting = true;
        HandleInteraction();
    }

    void HandleInteraction()
    {
        if (string.IsNullOrEmpty(characterID))
        {
            Debug.LogError("NPC sin characterID");
            ResetInteraction();
            return;
        }

        // Afinidad + UI
        if (AffinityChoiceHandler.Instance != null)
            AffinityChoiceHandler.Instance.currentCharacterID = characterID;

        AffinityUI.Instance?.SetCharacter(characterID);

        int stage = CharacterProgress.Instance.GetProgress(characterID);

        switch (stage)
        {
            // =========================
            // STAGE 0
            // =========================
            case 0:

                if (!GameProgressManager.Instance.IsUnlocked(minigame1ID))
                {
                    PlayDialogue(dialogueStage1, () =>
                    {
                        GameProgressManager.Instance.UnlockMinigame(minigame1ID);
                    });
                }
                else if (!GameProgressManager.Instance.IsCompleted(minigame1ID))
                {
                    PlayDialogue(goToMinigame1);
                }
                else
                {
                    CharacterProgress.Instance.IncreaseProgress(characterID);
                    ResetInteraction();
                }

                break;

            // =========================
            // STAGE 1
            // =========================
            case 1:

                if (!GameProgressManager.Instance.IsUnlocked(minigame2ID))
                {
                    PlayDialogue(dialogueStage2, () =>
                    {
                        GameProgressManager.Instance.UnlockMinigame(minigame2ID);
                    });
                }
                else if (!GameProgressManager.Instance.IsCompleted(minigame2ID))
                {
                    PlayDialogue(goToMinigame2);
                }
                else
                {
                    CharacterProgress.Instance.IncreaseProgress(characterID);
                    ResetInteraction();
                }

                break;

            // =========================
            // STAGE 2
            // =========================
            case 2:

                if (!GameProgressManager.Instance.IsUnlocked(minigame3ID))
                {
                    PlayDialogue(dialogueStage3, () =>
                    {
                        GameProgressManager.Instance.UnlockMinigame(minigame3ID);
                    });
                }
                else if (!GameProgressManager.Instance.IsCompleted(minigame3ID))
                {
                    PlayDialogue(goToMinigame3);
                }
                else
                {
                    CharacterProgress.Instance.IncreaseProgress(characterID);
                    ResetInteraction();
                }

                break;

            // =========================
            // FINAL
            // =========================
            default:
                LaunchEnding();
                break;
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

        ResetInteraction();
    }

    // =========================
    // DIÁLOGO
    // =========================

    void PlayDialogue(DialogueSceneConfig dialogue, System.Action onComplete = null)
    {
        if (KMA_DialogueManager.Instance == null)
        {
            Debug.LogError("KMA_DialogueManager no encontrado");
            ResetInteraction();
            return;
        }

        var window = KMA_DialogueManager.Instance.dialogueWindow as NovelSampleDialogueWindow;

        if (window == null)
        {
            Debug.LogError("DialogueWindow incorrecto");
            ResetInteraction();
            return;
        }

        if (dialogue == null)
        {
            Debug.LogError("DialogueSceneConfig no asignado");
            ResetInteraction();
            return;
        }

        window.GetType()
            .GetField("_dialogueScenes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(window, new List<DialogueSceneConfig> { dialogue });

        KMA_DialogueManager.Instance.StartDialogue();

        //  Esperar a que termine el diálogo
        StartCoroutine(WaitForDialogueEnd(onComplete));
    }

    IEnumerator WaitForDialogueEnd(System.Action onComplete)
    {
        // Espera a que el diálogo INICIE
        yield return new WaitForSeconds(0.2f);

        // Espera a que el diálogo termine realmente (cuando UI se oculta)
        while (KMA_DialogueManager.Instance != null &&
               KMA_DialogueManager.Instance.dialogueWindow.gameObject.activeSelf)
        {
            yield return null;
        }

        onComplete?.Invoke();
        ResetInteraction();
    }

    void ResetInteraction()
    {
        isInteracting = false;
    }
}