using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New DialogueGroup", menuName = "Mission/New Dialogue Group")]
public class DialogueGroupSO : ScriptableObject
{
    public int ID;
    public string triggerNpcID;
    public List<DialogueParagraphSO> paragrapgSos;
}
