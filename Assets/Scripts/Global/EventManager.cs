using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<string> OnUnlockMission;

    public static void OnTriggerUnlockMission(string missionID)
    {
        OnUnlockMission?.Invoke(missionID);
    }
}
