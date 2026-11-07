using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<string, int> OnConditionChange;             //更改限制值事件；

    public static event Action<string> OnAcceptMission;                  //接取任务事件；

    /// <summary>
    /// 传入限制的名称，设置对应的限制的值；
    /// </summary>
    /// <param name="missionID"></param>
    public static void OnTriggerConditionChange(string conditionID, int value) => OnConditionChange?.Invoke(conditionID, value);
    
    /// <summary>
    /// 传入任务的ID，接取对应的任务；
    /// </summary>
    /// <param name="missionID"></param>
    public static void OnTriggerAcceptMission(string missionID) => OnAcceptMission?.Invoke(missionID);

    public static event Action<bool> OnPauseGame;

    public static void OnTriggerPauseGame(bool isPause)
    {
        OnPauseGame?.Invoke(isPause);
    }

    public static event Action<int> OnSwitchDaily;              //切换闲聊事件；

    /// <summary>
    /// 切换日常闲聊对话；
    /// </summary>
    /// <param name="index"></param>
    public static void OnTriggerSwitchDaily(int index)
    {
        OnSwitchDaily?.Invoke(index);
    }


}
