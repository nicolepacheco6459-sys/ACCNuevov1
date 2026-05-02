using UnityEngine;
using KissMyAssets.VisualNovelCore.Runtime;

public class NPC_PauseMovement : MonoBehaviour
{
    private WaypointMover waypointMover;
    private Animator animator;
    private Rigidbody2D rb2D;
    private bool dialoguePaused;

    void Awake()
    {
        waypointMover = GetComponent<WaypointMover>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool shouldPause = PauseController.IsGamePaused || dialoguePaused || IsKmaDialogueActive();

        if (waypointMover != null)
        {
            waypointMover.enabled = !shouldPause;
        }

        if (rb2D != null && shouldPause && rb2D.bodyType != RigidbodyType2D.Static)
        {
            rb2D.linearVelocity = Vector2.zero;
        }

        if (animator != null && shouldPause)
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void SetDialoguePause(bool active)
    {
        dialoguePaused = active;
    }

    private bool IsKmaDialogueActive()
    {
        if (KMA_DialogueManager.Instance == null)
            return false;

        var window = KMA_DialogueManager.Instance.dialogueWindow;
        return window != null && window.gameObject.activeSelf;
    }
}
