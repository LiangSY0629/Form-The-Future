using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Line")]
public class DialogueLineSO : ScriptableObject
{
    public int ID;
    public string dialogueName;
    [TextArea]public string dialogueText;
    public int nextID;

    public List<DialogueChoose> dialogueChooses;
    [SerializeReference]public List<DialogueEffectSO> dialogueEffects;

}

[Serializable]
public struct DialogueChoose
{
    public string chooseText;
    public int nextID;
}
