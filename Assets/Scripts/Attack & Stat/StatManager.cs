using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;

    public List<Stat> stats;

    public Dictionary<string, StatRunTime> statDictionary;

    public StatViewController statViewController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        statDictionary = new Dictionary<string, StatRunTime>();

        foreach(Stat stat in stats)
        {
            StatRunTime statRun = new()
            {
                ID = stat.ID,
                baseValue = stat.value,
            };

            statRun.SetFinalValue();

            statDictionary[stat.ID] = statRun;
        }

    }
    
    /// <summary>
    /// 给对应属性添加加成；
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="modifier"></param>
    public void AddStatModirier(string ID, StatModifier modifier)
    {
        if (!statDictionary.TryGetValue(ID, out StatRunTime statRun))
            return;

        statRun.AddModifier(modifier);
    }

    /// <summary>
    /// 移除对应属性的加成；
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="modifier"></param>
    public void RemoveStatModifier(string ID, StatModifier modifier)
    {
        if (!statDictionary.TryGetValue(ID, out StatRunTime statRun))
            return;

        statRun.RemoveModifier(modifier);
    }

    /// <summary>
    /// 传入ID和值，更改属性的基础值；
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="value"></param>
    public void StatBaseModifier(string ID, float value)
    {
        if (!statDictionary.TryGetValue(ID, out StatRunTime statRun))
            return;

        statRun.baseValue += value;
        statRun.SetFinalValue();

        if(statViewController != null)
            statViewController.SetStatView(ID);

    }


}

[Serializable]
public struct Stat
{
    public string ID;
    public float value;
    public float addOnUp;
}


/// <summary>
/// 当前属性的运行状态；
/// </summary>
public class StatRunTime
{
    public string ID;

    public float baseValue;

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
        if (modifierDictionary.TryGetValue(modifier.sourceID, out StatModifier oldModifier))
            modifierDictionary.Remove(modifier.sourceID);

        SetFinalValue();
    }

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