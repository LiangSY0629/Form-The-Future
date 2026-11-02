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

    public bool CanDialogue()
    {
        if(MissioneManager.Instance != null)
        {
            if (MissioneManager.Instance.HaveMission(NpcName))
                return true;
        }

        return false;
    }

}
