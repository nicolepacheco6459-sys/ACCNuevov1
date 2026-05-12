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

    [Header("Finales Masculino")]
    public DialogueSceneConfig maleGoodEnding;
    public DialogueSceneConfig maleNeutralEnding;
    public DialogueSceneConfig maleBadEnding;

    [Header("Finales Femenino")]
    public DialogueSceneConfig femaleGoodEnding;
    public DialogueSceneConfig femaleNeutralEnding;
    public DialogueSceneConfig femaleBadEnding;

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

        if (AffinityChoiceHandler.Instance != null)
            AffinityChoiceHandler.Instance.currentCharacterID = characterID;

        AffinityUI.Instance?.SetCharacter(characterID);

        int stage = CharacterProgress.Instance.GetProgress(characterID);

        Debug.Log($"📊 {characterID} está en stage {stage}");

        switch (stage)
        {
            // =========================
            // STAGE 0
            // =========================
            case 0:

                if (!GameProgressManager.Instance.IsUnlocked(minigame1ID))
                {
                    Debug.Log("🎬 Iniciando diálogo Stage 1");

                    PlayDialogue(dialogueStage1);

                    // 🔥 DESBLOQUEO SEGURO
                    StartCoroutine(UnlockAfterDialogue(minigame1ID));
                }
                else if (!GameProgressManager.Instance.IsCompleted(minigame1ID))
                {
                    Debug.Log("📍 Mostrando diálogo de ir al minijuego 1");

                    PlayDialogue(goToMinigame1);
                }
                else
                {
                    Debug.Log("⬆️ Avanzando a Stage 1");

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
                    Debug.Log("🎬 Iniciando diálogo Stage 2");

                    PlayDialogue(dialogueStage2);

                    StartCoroutine(UnlockAfterDialogue(minigame2ID));
                }
                else if (!GameProgressManager.Instance.IsCompleted(minigame2ID))
                {
                    Debug.Log("📍 Mostrando diálogo de ir al minijuego 2");

                    PlayDialogue(goToMinigame2);
                }
                else
                {
                    Debug.Log("⬆️ Avanzando a Stage 2");

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
                    Debug.Log("🎬 Iniciando diálogo Stage 3");

                    PlayDialogue(dialogueStage3);

                    StartCoroutine(UnlockAfterDialogue(minigame3ID));
                }
                else if (!GameProgressManager.Instance.IsCompleted(minigame3ID))
                {
                    Debug.Log("📍 Mostrando diálogo de ir al minijuego 3");

                    PlayDialogue(goToMinigame3);
                }
                else
                {
                    Debug.Log("⬆️ Avanzando a FINAL");

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
    // DESBLOQUEO SEGURO
    // =========================
    IEnumerator UnlockAfterDialogue(string minigameID)
    {
        yield return new WaitForSeconds(1.5f);

        Debug.Log("🔥 DESBLOQUEANDO MINIJUEGO: " + minigameID);

        GameProgressManager.Instance.UnlockMinigame(minigameID);
    }

    // =========================
    // FINALES
    // =========================
    void LaunchEnding()
    {
        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        bool isFemale = PlayerCharacterData.IsFemale();

        Debug.Log(" Afinidad final: " + affinity);
        Debug.Log(" Género jugador: " + (isFemale ? "Femenino" : "Masculino"));

        // =========================
        // GOOD ENDING
        // =========================
        if (affinity >= 50)
        {
            if (isFemale)
                PlayDialogue(femaleGoodEnding);
            else
                PlayDialogue(maleGoodEnding);
        }

        // =========================
        // NEUTRAL ENDING
        // =========================
        else if (affinity >= 20)
        {
            if (isFemale)
                PlayDialogue(femaleNeutralEnding);
            else
                PlayDialogue(maleNeutralEnding);
        }

        // =========================
        // BAD ENDING
        // =========================
        else
        {
            if (isFemale)
                PlayDialogue(femaleBadEnding);
            else
                PlayDialogue(maleBadEnding);
        }

        ResetInteraction();
    }

    // =========================
    // DIÁLOGO
    // =========================
    void PlayDialogue(DialogueSceneConfig dialogue)
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
    }

    void ResetInteraction()
    {
        isInteracting = false;
    }
}