using System;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;

    public List<Stat> stats;

    public Dictionary<StatTypes, StatRunTime> statRunDictionary;

    bool isStatChange;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        statRunDictionary = new Dictionary<StatTypes, StatRunTime>();

        foreach(Stat stat in stats)
        {
            StatRunTime statRun = new()
            {
                type = stat.type,
                baseValue = stat.value,
            };

            statRun.SetFinalValue();

            statRunDictionary[stat.type] = statRun;
        }

    }

    private void LateUpdate()
    {
        //判断要不要刷新，如果需要，判断有没有显示层，有就更新并刷新；
        if (!isStatChange)
            return;

        //属性值发生了更改；
        EventManager.OnTriggerStatChanged();

        isStatChange = false;
    }

    /// <summary>
    /// 传入ID和值，更改属性的基础值；
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void StatBaseModifier(StatTypes type, float value)
    {
        if (!statRunDictionary.TryGetValue(type, out StatRunTime statRun))
            return;

        statRun.baseValue += value;
        statRun.SetFinalValue();

        isStatChange = true;
    }

}

[Serializable]
public struct Stat
{
    public StatTypes type;
    public float value;
    public float baseModifier;
}


/// <summary>
/// 当前属性的运行状态；
/// </summary>
public class StatRunTime
{
    public StatTypes type;

    public float baseValue;

    public float currentValue;

    public float finalValue;

    public Dictionary<string, StatModifier> modifierDictionary = new();

    /// <summary>
    /// 计算最终值；
    /// </summary>
    public void SetFinalValue()
    {
        float directValue = 0;
        float percentValue = 0;

        foreach(StatModifier modifier in modifierDictionary.Values)
        {
            directValue += modifier.directValue;
            percentValue += modifier.percentValue;
        }

         finalValue = directValue + baseValue * (1 + percentValue);
    }

    /// <summary>
    /// 添加对应ID的加成；
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(StatModifier modifier)
    {
        modifierDictionary[modifier.sourceID] = modifier;

        SetFinalValue();
    }

    /// <summary>
    /// 移除对应ID的加成；
    /// </summary>
    /// <param name="sourceID"></param>
    public void RemoveModifier(StatModifier modifier)
    {
        modifierDictionary.Remove(modifier.sourceID);

        SetFinalValue();
    }

}


[Serializable]
public class Buff
{

}


/// <summary>
/// 属性的加成；
/// </summary>
public class StatModifier
{
    public string sourceID;

    public float directValue;

    public float percentValue;
}