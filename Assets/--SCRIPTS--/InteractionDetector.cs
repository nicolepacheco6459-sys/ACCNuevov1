using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    [Header("Input (optional)")]
    [Tooltip("Optional: assign a PlayerInput in the scene or leave blank to auto-detect.")]
    public UnityEngine.InputSystem.PlayerInput playerInput;

    private UnityEngine.InputSystem.InputAction boundAction;

    void Start()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(false);

        // Try to auto-bind to PlayerInput.actions -> "Interact" action
        if (playerInput == null)
            playerInput = FindFirstObjectByType<UnityEngine.InputSystem.PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            var act = playerInput.actions.FindAction("Interact");
            if (act != null)
            {
                BindAction(act);
            }
        }
    }

    private void OnEnable()
    {
        // if PlayerInput assigned later, try to bind
        if (playerInput != null && boundAction == null && playerInput.actions != null)
        {
            var act = playerInput.actions.FindAction("Interact");
            if (act != null) BindAction(act);
        }
    }

    private void OnDisable()
    {
        UnbindAction();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            interactableInRange?.Interact();
        }
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactableInRange?.Interact();
        }
    }

    private void BindAction(UnityEngine.InputSystem.InputAction action)
    {
        if (action == null) return;
        boundAction = action;
        boundAction.performed += OnBoundActionPerformed;
        boundAction.Enable();
    }

    private void UnbindAction()
    {
        if (boundAction == null) return;
        boundAction.performed -= OnBoundActionPerformed;
        boundAction.Disable();
        boundAction = null;
    }

    private void OnBoundActionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        interactableInRange?.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            if (interactionIcon != null)
                interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            if (interactionIcon != null)
                interactionIcon.SetActive(false);
        }
    }


}
