using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MissioneManager : MonoBehaviour
{
    public static MissioneManager Instance;

    public DialogueViewController[] dialogueView;  //拖入Dialogue View Controller的子类；

    [HideInInspector]public List<string> currentMission;   //存储所有已解锁的任务；

    [Header("拖入所有的任务")]
    public List<MissionSO> MissionSos;    //存储所有任务的SO；
    public Dictionary<string, MissionRunTime> missionDictionary;    //根据任务ID查询当前任务状态；

    [Header("设置一些相关的信息")]
    public Color fadeColor;
    public float fadeTime;

    [HideInInspector] public bool isInFade = false;

    DialogueSession currentSession;
    readonly List<DialogueChoose> dailyChoose = new();

    /// <summary>
    /// 创建任务的运行状态；
    /// </summary>
    public class MissionRunTime
    {
        public string missionID;               //任务ID；
        public int groupIndex;               //当前任务进度；
        public bool complete;               //是否可完成；

        public MissionSO mission;                                         //当前RunTime所对应的Mission；
        public DialogueGroupSO Group => mission.groupSOs[groupIndex];     //当前任务的对话组；
        public string TriggerNpcID => Group.triggerNpcID;                 //当前任务触发需要的NpcID；
    }

    /// <summary>
    /// 当前正在运行的对话的暂存数据；
    /// </summary>
    public class DialogueSession
    {
        public DialogueViewController view;

        public MissionRunTime missionRun;
        public DialogueGroupSO group => missionRun.Group;
        public DialogueParagraphSO paragraph => group.paragrapgSos[paragraphIndex];
        public DialogueLine line => paragraph.dialogueLines[lineIndex];

        public int paragraphIndex;    //对话段的索引；
        public int lineIndex;          //对话句的索引

    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        //运行时创建所有的mission run time，并添加进字典；
        missionDictionary = new Dictionary<string, MissionRunTime>();

        foreach (MissionSO missionSO in MissionSos)
        {
            //如果需要为group创建顺序ID，就取消注释；
            //for(int i = 0; i < mission.groupSOs.Count; i++)
            //{
            //    mission.groupSOs[i].ID = i;
            //}


            MissionRunTime missionRun = new()
            {
                missionID = missionSO.missionID,
                //groupIndex = -1,
                groupIndex = 0,
                complete = false,
                mission = missionSO
            };

            if (missionRun.groupIndex == 0)
                currentMission.Add(missionRun.missionID);
            missionDictionary[missionRun.missionID] = missionRun;

        }

    }


    private void OnEnable()
    {
        EventManager.OnUnlockMission += UnlockMission;       //订阅任务解锁事件；
    }


    private void Start()
    {
        //运行时从存档内获取进度，并赋值到mission runtime上；

    }

    private void OnDisable()
    {
        EventManager.OnUnlockMission -= UnlockMission;     //取消订阅任务解锁事件；
    }


    /// <summary>
    /// 传入ID判断有没有跟这个npc相关的任务；
    /// </summary>
    /// <param name="NpcID"></param>
    /// <returns></returns>
    public bool HaveMission(string NpcID)
    {
        if (currentSession != null)
            return false;

        //遍历已解锁的任务，如果没找到对应的任务，就返回false，找到则返回true；
        foreach (string ID in currentMission)
        {
            if (missionDictionary[ID].TriggerNpcID == NpcID)
                return true;
        }

        return false;
    }



    /// <summary>
    /// 传入日常对话，显示传入的日常对话；
    /// </summary>
    /// <param name="daily"></param>
    public void SetDailyDialogue(MissionSO daily, string npcID)
    {
        string currentID = "";

        //判断传入的daily是不是null，daily的type是不是daily；
        if (daily != null && daily.missionType == MissionTypes.Daily)
        {

            Debug.Log("daily");
            currentID = daily.missionID;

            dailyChoose.Clear();

            //遍历所有的已解锁任务，如果有就把这个当作选项；
            foreach(string ID in currentMission)
            {
                if (missionDictionary[ID].TriggerNpcID == npcID)
                {
                    DialogueChoose choose = new()
                    {
                        chooseText = ID,
                        chooseType = ChooseTypes.Mission,
                        nextID = ID,
                    };

                    dailyChoose.Add(choose);
                }
            }

        }
        else
        {
            foreach (string ID in currentMission)
            {
                Debug.Log(ID);
                if (missionDictionary[ID].TriggerNpcID == npcID)       //从前往后找，找到匹配的就终止，后续改为用list全部显示；
                {
                    currentID = ID;
                    break;
                }

            }

        }

        MissionRunTime missionRun;

        //判断有没有这个任务的运行状态，防止出错；
        if (missionDictionary.ContainsKey(currentID))       
            missionRun = missionDictionary[currentID];
        else
            missionRun = new()
            {
                missionID = daily.missionID,
                mission = daily,
                groupIndex = 0,
                complete = false,
            };


        DialogueViewController view = GetView(missionRun.Group);

        DialogueSession session = new()
        {
            view = view,
            missionRun = missionRun,
            lineIndex = 0,
            paragraphIndex = 0,
        };

        currentSession = session;

        currentSession.view.OpenView();
        SetLine();

    }

    /// <summary>
    /// 获取传入对话组对应的View;
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    DialogueViewController GetView(DialogueGroupSO group)
    {
        if (!group.isSubtitle)        //判断使用哪一种view方式；
            return dialogueView[0];
        else
            return dialogueView[1];
    }


    /// <summary>
    /// 调用函数判断该不该显示选项，还是只显示对话；（防止出现对话段出现只有一句对话但无法及时显示选项）
    /// </summary>
    public void SetLine()
    {
        currentSession.view.SetLine(currentSession.line);

        if (currentSession.lineIndex + 1 >= currentSession.paragraph.dialogueLines.Count)
        {
            List<DialogueChoose> currentChoose = new();

            //判断对话段有没有选项，有就显示；
            if (currentSession.paragraph.dialogueChooses.Count > 0)
            {
                //判断当前对话是不是daily对话，且daily Choose不为空；
                if (currentSession.missionRun.mission.missionType == MissionTypes.Daily && dailyChoose.Count > 0)
                {
                    foreach (DialogueChoose choose in dailyChoose)
                        currentChoose.Add(choose);
                }

                foreach (DialogueChoose choose in currentSession.paragraph.dialogueChooses)
                    currentChoose.Add(choose);
            }

            //判断有没有选项，有就显示选项；
            if (currentChoose.Count > 0)
            {
                IReadOnlyList<DialogueChoose> chooses = currentChoose;
                currentSession.view.SetChoose(chooses);
            }

        }
    }


    /// <summary>
    /// 切换下一句，如果没有下一句，显示选项，如果没有下一句也没有选项，关闭对话；
    /// </summary>
    public void NextLine()
    {

        //先判断还剩多少句，如果只剩一句，就播放最后一句的同时显示选项；如果没有选项可供显示，那么将会在下一次点击关闭该对话组；
        if (currentSession.lineIndex + 2 <= currentSession.paragraph.dialogueLines.Count)
        {
        
            currentSession.lineIndex++;
            SetLine();
        }
        else
        {
            if (currentSession.paragraph.dialogueChooses.Count <= 0)
                OnGroupEnd();
        }
        
    }


    /// <summary>
    /// 对话组结束时调用，判断该不该接下一对话组，额外附加淡入淡出效果；
    /// </summary>
    public async void OnGroupEnd()
    {
        isInFade = true;

        await SceneFade.Instance.FadeIn(fadeColor, fadeTime);

        if (currentSession.missionRun.groupIndex + 1 < currentSession.missionRun.mission.groupSOs.Count)       //不会超出索引再加；
        {
            currentSession.missionRun.groupIndex++;
        }
        else
        {

            currentSession.view.Close();
            currentSession.missionRun.complete = true;
            currentMission.Remove(currentSession.missionRun.missionID);

            currentSession = null;

            await SceneFade.Instance.FadeOut(fadeColor, fadeTime);
            isInFade = false;
            return;
        }

        if (currentSession.group.isContinuous)             //如果当前对话组是连续的，就连续播放下一对话组；
        {
            currentSession.view.Close();

            //更新session的数据，开始下一段；
            currentSession.view = GetView(currentSession.group);
            currentSession.paragraphIndex = 0;
            currentSession.lineIndex = 0;

            currentSession.view.OpenView();
            SetLine();
        }
        else
        {
            currentSession.view.Close();
            currentSession = null;
        }

        await SceneFade.Instance.FadeOut(fadeColor, fadeTime);

        isInFade = false;

    }


    /// <summary>
    /// 传入对应的对话段ID，切换到对应的对话段；
    /// </summary>
    /// <param name="paragraphID"></param>
    public void GoToParagraph(string paragraphID)
    {
        for(int i = 0; i < currentSession.group.paragrapgSos.Count; i++)             //循环比对是否有对应的对话段，有就更改对话段索引并重置对话句索引，否则不做更改；
        {
            if (currentSession.group.paragrapgSos[i].paragraphID == paragraphID)
            {
                currentSession.paragraphIndex = i;
                currentSession.lineIndex = 0;
                break;
            }

        }

        SetLine();

    }

    /// <summary>
    /// 传入对话组的ID，跳转到对应组；
    /// </summary>
    /// <param name="groupID"></param>
    public void GoToGroup(string groupID)
    {
        for(int i = 0;i < currentSession.missionRun.mission.groupSOs.Count; i++)
        {
            if (currentSession.missionRun.mission.groupSOs[i].groupID == groupID)
            {
                currentSession.missionRun.groupIndex = i;
                currentSession.paragraphIndex = 0;
                currentSession.lineIndex = 0;
                break;
            }
        }

        SetLine();

    }


    /// <summary>
    /// 传入任务的ID，跳转到对应的任务组；
    /// </summary>
    /// <param name="missionID"></param>
    public void GoToMission(string missionID)
    {
        if (!missionDictionary.TryGetValue(missionID, out MissionRunTime missionRun))
        {
            currentSession.view.Close();
            return;
        }


        currentSession.missionRun = missionRun;
        currentSession.paragraphIndex = 0;
        currentSession.lineIndex = 0;
        currentSession.view = GetView(currentSession.group);

        SetLine();

    }



    /// <summary>
    /// 传入任务ID解锁对应任务；
    /// </summary>
    /// <param name="missionID"></param>
    public void UnlockMission(string missionID)
    {
        currentMission.Add(missionID);
        missionDictionary[missionID].groupIndex = 0;
    }


}
