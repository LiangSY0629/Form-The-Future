using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<string, float> OnConditionChange;             //更改限制值事件；

    /// <summary>
    /// 传入限制的名称，设置对应的限制的值；
    /// </summary>
    /// <param name="missionID"></param>
    public static void OnTriggerConditionChange(string conditionID, float value) => OnConditionChange?.Invoke(conditionID, value);
}
