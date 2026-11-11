using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcInteracte : MonoBehaviour,IDialogueNPC
{
    public string NpcName;

    [Header("放入闲聊对话")]
    public MissionSO[] dailyDialogue;
    public int dailyIndex;

    //private void OnEnable()
    //{
    //    EventManager.OnSwitchDaily += SwitchDaily;
    //}

    //private void OnDisable()
    //{
    //    EventManager.OnSwitchDaily -= SwitchDaily;
    //}

    public string DialogueNPC()
    {
        return NpcName;
    }

    public MissionSO DailyDialogue()
    {
        if (dailyDialogue.Length > 0 && 0 <= dailyIndex && dailyIndex < dailyDialogue.Length)
        {
            return dailyDialogue[dailyIndex];
        }

        return null;
    }

    public bool CanDialogue()
    {
        //如果有闲聊，直接返回true；
        if(dailyDialogue.Length > 0 && 0 <= dailyIndex && dailyIndex < dailyDialogue.Length)
            return true;

        if(MissioneManager.Instance != null)
        {
            if (MissioneManager.Instance.HaveMission(NpcName))
                return true;
        }

        return false;
    }


    //void SwitchDaily(int index) => dailyIndex = index;


}
