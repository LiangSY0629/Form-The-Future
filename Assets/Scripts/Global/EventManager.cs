using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<string> OnUnlockMission;             //解锁任务事件；

    /// <summary>
    /// 传入任务的名称，解锁对应的任务；
    /// </summary>
    /// <param name="missionID"></param>
    public static void OnTriggerUnlockMission(string missionID)
    {
        OnUnlockMission?.Invoke(missionID);
    }

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
