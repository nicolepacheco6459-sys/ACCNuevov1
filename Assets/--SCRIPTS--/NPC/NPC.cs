using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool IsDialogueActive => isDialogueActive;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else
        {
            NextLine();
        }

    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        dialogueUI.HideChoices();
        PauseController.SetPause(true);

        StartCoroutine(Typeline());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            dialogueUI.HideChoices();
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(Typeline());
        }
        else
        {
            EndDialogue();
        }
        
    }

    IEnumerator Typeline()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Check for choices
        DialogueChoise currentChoice = GetChoiceForIndex(dialogueIndex);
        if (currentChoice != null)
        {
            dialogueUI.ShowChoices(currentChoice.choices, OnChoiceSelected);
        }
        else if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.HideChoices();
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    private DialogueChoise GetChoiceForIndex(int index)
    {
        foreach (DialogueChoise choice in dialogueData.choices)
        {
            if (choice.dialogueIndex == index)
                return choice;
        }
        return null;
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        DialogueChoise currentChoice = GetChoiceForIndex(dialogueIndex);
        if (currentChoice != null && choiceIndex < currentChoice.nextDialogueIndexes.Length)
        {
            dialogueIndex = currentChoice.nextDialogueIndexes[choiceIndex] - 1; // -1 because NextLine increments
            dialogueUI.HideChoices();
            NextLine();
        }
    }
}
