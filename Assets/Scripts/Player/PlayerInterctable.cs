using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInterctable : MonoBehaviour
{
    IInteractable currentInterction = null;
    IDialogueNPC currentNPC = null;

    public GameObject interactionIcon;

    private void Start()
    {
        interactionIcon.SetActive(false);
    }

    /// <summary>
    /// 尝试调用接口进行交互；
    /// </summary>
    public void TryInteractable(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            Debug.Log("yes");
            currentInterction?.Interactable();
            if (currentNPC != null && DialogueManager.Instance != null)
                DialogueManager.Instance.SetDialogue(currentNPC.DialogueNPC());
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interctable) && interctable.CanInteracte())
        {
            currentInterction = interctable;
            interactionIcon.SetActive(true);
        }
        else if(other.TryGetComponent(out IDialogueNPC dialogueNPC))
        {
            currentNPC = dialogueNPC;
            interactionIcon.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interctable) && currentInterction == interctable)
        {
            currentInterction = null;
            interactionIcon.SetActive(false);
        }
        else if(other.TryGetComponent(out IDialogueNPC _))
        {
            currentNPC = null;
            interactionIcon.SetActive(false);
        }

    }

}
