using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //public int version;
    public string dataName;
    public long saveAt;
    public List<Data> datas;
}

[System.Serializable]
public class Data
{
    public string key;

    public string value;
}


[System.Serializable]
public class MissionSaveData
{
    public string trickMissionID;
    public List<MissionData> missionDatas;
    public List<ConditionData> conditionDatas;

}

[System.Serializable]
public class MissionData
{
    public string missionID;
    public MissionStates missionState;
    public int groupIndex;
}

[System.Serializable]
public class ConditionData
{
    public string conditionID;
    public float value;
}