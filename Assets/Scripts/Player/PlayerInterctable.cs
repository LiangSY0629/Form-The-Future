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
            currentInterction?.Interactable();
            if (currentNPC != null && MissioneManager.Instance != null)
            {
                MissionSO daily = currentNPC.DailyDialogue();
                MissioneManager.Instance.SetDailyDialogue(daily, currentNPC.DialogueNPC());
                interactionIcon.SetActive(false);
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out IInteractable interctable) && interctable.CanInteracte())
        {
            currentInterction = interctable;
            interactionIcon.SetActive(true);
        }
        else if(other.TryGetComponent(out IDialogueNPC dialogueNPC) && dialogueNPC.CanDialogue())
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
        else if(other.TryGetComponent(out IDialogueNPC npc) && currentNPC == npc)
        {
            currentNPC = null;
            interactionIcon.SetActive(false);
        }

    }

}
