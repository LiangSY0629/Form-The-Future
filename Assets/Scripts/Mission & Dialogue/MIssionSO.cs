using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "New Mission", menuName = "Mission/New Mission")]
public class MissionSO : ScriptableObject
{
    public string missionID;                    //任务的ID，具有唯一性；

    public MissionTypes missionType;            //任务的类型（判断是不是主线，支线等任务）

    public bool accepted;                    //判断是否需要接取任务；

    [TextArea] public string description;       //任务的描述；

    public List<DialogueGroupSO> groupSOs;      //任务的所有主对话组（即接取任务后的对话组）；

    public List<UnlockCondition> conditions;    //任务的前置解锁条件，必须全部完成才能解锁任务；
}

/// <summary>
/// 解锁条件
/// </summary>
[Serializable]
public class UnlockCondition
{
    //限制的ID；
    public string conditionID;

    //解锁限制的值；
    public int value;
}