using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Paragraph", menuName = "Mission/New Dialogue Paragreph")]
public class DialogueParagraphSO : ScriptableObject
{
    public string paragraphID;
    public List<DialogueLine> dialogueLines;

    public List<DialogueChoose> dialogueChooses;

}

[Serializable]
public class DialogueChoose
{
    public string chooseText;
    public ChooseTypes chooseType;
    public string nextID;
}