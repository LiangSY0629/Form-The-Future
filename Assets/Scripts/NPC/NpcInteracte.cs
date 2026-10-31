using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInteracte : MonoBehaviour,IDialogueNPC
{
    public string NpcName;


    public string DialogueNPC()
    {
        return NpcName;
    }
}
