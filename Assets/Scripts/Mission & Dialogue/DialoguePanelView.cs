using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialoguePanelView : DialogueViewController
{
    public GameObject dialoguePanel;
    public Transform chooseTransform;
    public GameObject dialogueChoose;

    public TMP_Text dialogueName, dialogueText;

    bool isChoose = false;
    bool isSpeaking = false;
    string currentLine;

    Coroutine coroutine;

    public override void OpenView()
    {
        dialoguePanel.SetActive(true);
        GameManager.Instance.SetInputMap("UI", true);
    }


    public override void SetLine(DialogueLine line)
    {
        dialogueName.text = line.dialogueName;
        //dialogueText.text = line.dialogueText;
        currentLine = line.dialogueText;

        coroutine = StartCoroutine(SetLetter());

    }

    public override void SetChoose(IReadOnlyList<DialogueChoose> chooses)
    {
        for(int i = 0; i< chooses.Count; i++)
        {
            GameObject choose = Instantiate(dialogueChoose, chooseTransform);
            choose.GetComponentInChildren<TMP_Text>().text = chooses[i].chooseText;

            string targetID = chooses[i].nextID;
            ChooseTypes type = chooses[i].chooseType;

            choose.GetComponent<Button>().onClick.AddListener(() => OnPressedChoose(type, targetID));
            choose.SetActive(false);
        }

        isChoose = true;
    }

    public override void Close()
    {
        dialoguePanel.SetActive(false);
        GameManager.Instance.SetInputMap("Player", false);
    }


    public override void InputNextLine(InputAction.CallbackContext action)
    {
        if (action.performed && isSpeaking)
        {
            isSpeaking = false;
            StopCoroutine(coroutine);

            dialogueText.text = currentLine;

            if (isChoose)
            {
                foreach (Transform transform in chooseTransform)
                {
                    transform.gameObject.SetActive(true);
                }

            }

            return;
        }


        if (action.performed && dialoguePanel.activeSelf && !MissioneManager.Instance.isInFade && !isChoose)
        {
            MissioneManager.Instance.NextLine();
        }
    }

    public override void OnPressedChoose(ChooseTypes type, string targetID)
    {
        foreach (Transform choose in chooseTransform)
        {
            choose.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(choose.gameObject);
        }

        isChoose = false;

        switch (type)
        {
            case ChooseTypes.Mission : MissioneManager.Instance.GoToMission(targetID);
                break;
            case ChooseTypes.Group : MissioneManager.Instance.GoToGroup(targetID);
                break;
            case ChooseTypes.Paragraph : MissioneManager.Instance.GoToParagraph(targetID);
                break;
        }


    }


    /// <summary>
    /// 传入line，逐字显示文本；
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private IEnumerator  SetLetter()
    {
        isSpeaking = true;
        dialogueText.text = "";

        foreach(char letter in currentLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }

        if (isChoose)
        {
            foreach (Transform transform in chooseTransform)
            {
                transform.gameObject.SetActive(true);
            }

        }

        isSpeaking = false;
    }



}
