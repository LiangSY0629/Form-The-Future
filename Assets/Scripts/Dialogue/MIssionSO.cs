using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "New Mission", menuName = "Mission/New Mission")]
public class MissionSO : ScriptableObject
{
    public string missionID;

    public MissionTypes missionType;

    public List<DialogueGroupSO> groupSOs;

}
