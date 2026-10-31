using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string dialogueName;
    [TextArea]public string dialogueText;
    [SerializeReference]public List<DialogueEffect> dialogueEffects;

}


