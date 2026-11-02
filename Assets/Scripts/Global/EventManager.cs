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

}
