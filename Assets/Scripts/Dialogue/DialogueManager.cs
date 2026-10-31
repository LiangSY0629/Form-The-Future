using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public List<string> currentMission;   //存储所有已解锁的任务；

    public List<MissionSO> MissionSos;    //存储所有任务的SO；

    public Dictionary<string, MissionSO> missionSoDictionary;       //根据任务ID查询当前任务的SO；
    public Dictionary<string, MissionRunTime> missionDictionary;    //根据任务ID查询当前任务状态；

    /// <summary>
    /// 创建任务的运行状态；
    /// </summary>
    public class MissionRunTime
    {
        public string missionID;               //任务ID；
        public MissionTypes missionType;       //任务类型；
        public int currentGroup;               //当前任务进度；
        public string triggerNpcID;            //当前任务触发需要的NpcID；
        public bool canComplete;               //是否可完成；

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }


    private void OnEnable()
    {
        EventManager.OnUnlockMission += UnlockMission;       //订阅任务解锁事件；
    }


    private void Start()
    {
        missionSoDictionary = new Dictionary<string, MissionSO>();
        missionDictionary = new Dictionary<string, MissionRunTime>();

        foreach(MissionSO mission in MissionSos)
        {
            MissionRunTime missionRun = new()
            {
                missionID = mission.missionID,
                missionType = mission.missionType,
                currentGroup = mission.groupSOs[0].ID,
                triggerNpcID = mission.groupSOs[0].triggerNpcID,
                canComplete = false,
            };

            missionSoDictionary[mission.missionID] = mission;
            missionDictionary[mission.missionID] = missionRun;

        }

    }

    private void OnDisable()
    {
        EventManager.OnUnlockMission -= UnlockMission;     //取消订阅任务解锁事件；
    }

    /// <summary>
    /// 传入当前的NpcID显示对应任务的对话，
    /// </summary>
    /// <param name="NpcID"></param>
    public void SetDialogue(string NpcID)
    {
        string currentID;

        foreach(string ID in currentMission)
        {
            if (missionDictionary[ID].triggerNpcID == NpcID)       //从前往后找，找到匹配的就终止，后续改为用list全部显示；
            {
                currentID = ID;
                break;
            }

        }

        //暂时为空，后续调用ui对话框，并填入当前选中的任务ID的当前组，开启对话。

    }


    public bool HaveMission(string NpcID)
    {
        //遍历已解锁的任务，如果没找到对应的任务，就返回false，找到则返回true；
        foreach(string ID in currentMission)
        {
            if (missionDictionary[ID].triggerNpcID == NpcID)
                return true;
        }

        return false;
    }


    /// <summary>
    /// 传入任务ID解锁对应任务；
    /// </summary>
    /// <param name="missionID"></param>
    public void UnlockMission(string missionID)
    {
        currentMission.Add(missionID);
        missionDictionary[missionID].canComplete = true;
    }


}
