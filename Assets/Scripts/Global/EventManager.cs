using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static event Action<string> OnMissionChanged;

    /// <summary>
    /// 传入任务名称，宣布这个任务完成；
    /// </summary>
    /// <param name="missionID"></param>
    /// <param name="stat"></param>
    public static void OnTriggerMissionChanged(string missionID) => OnMissionChanged?.Invoke(missionID);

    public static event Action<string, float> OnConditionChange;             //更改限制值事件；

    /// <summary>
    /// 传入限制的名称，设置对应的限制的值；
    /// </summary>
    /// <param name="missionID"></param>
    public static void OnTriggerConditionChange(string conditionID, float value) => OnConditionChange?.Invoke(conditionID, value);


    public static event Action OnStatChanged;

    /// <summary>
    /// 当当前属性发生更改时；
    /// </summary>
    public static void OnTriggerStatChanged() => OnStatChanged?.Invoke();

}
