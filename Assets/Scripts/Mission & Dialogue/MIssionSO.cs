using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "New Mission", menuName = "Mission/New Mission")]
public class MissionSO : ScriptableObject
{
    public string missionID;

    public MissionTypes missionType;

    public DialogueGroupSO preGroup;

    public List<DialogueGroupSO> groupSOs;

    public List<UnlockCondition> conditions;
}

[Serializable]
public class UnlockCondition
{
    //限制的ID；
    public string conditionID;

    //解锁限制的值；
    public int value;
}