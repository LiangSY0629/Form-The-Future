using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialoguePanelView : DialogueViewController
{
    public GameObject dialoguePanel;
    public Transform chooseTransform;
    public GameObject dialogueChoose;

    public TMP_Text dialogueName, dialogueText;

    public override void OpenView()
    {
        dialoguePanel.SetActive(true);
    }


    public override void SetLine(DialogueLine line)
    {
        dialogueName.text = line.dialogueName;
        dialogueText.text = line.dialogueText;

    }

    public override void SetChoose(IReadOnlyList<DialogueChoose> chooses)
    {
        for(int i = 0; i< chooses.Count; i++)
        {
            GameObject choose = Instantiate(dialogueChoose, chooseTransform);
            choose.GetComponentInChildren<TMP_Text>().text = chooses[i].chooseText;
            string targetID = chooses[i].nextID;
            choose.GetComponent<Button>().onClick.AddListener(() => OnPressedChoose(targetID));
        }
    }

    public override void Close()
    {
        dialoguePanel.SetActive(false);
    }


    public override void InputNextLine(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            Debug.Log("next");
            MissioneManager.Instance.NextLine();
        }
    }

    public override void OnPressedChoose(string paragraphID)
    {
        foreach (Transform choose in chooseTransform)
        {
            choose.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(choose.gameObject);
        }

        MissioneManager.Instance.GoToParagraph(paragraphID);

    }

}
