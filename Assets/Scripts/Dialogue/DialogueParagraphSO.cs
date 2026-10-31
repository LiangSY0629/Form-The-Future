using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New Paragraph", menuName = "Mission/New Dialogue Paragreph")]
public class DialogueParagraphSO : ScriptableObject
{
    public string paragraphID;
    public List<DialogueLine> dialogueParagraph;
    public bool isChoose;
    public List<DialogueChoose> dialogueChooses;

}

[Serializable]
public struct DialogueChoose
{
    public string chooseText;
    public int nextID;
}