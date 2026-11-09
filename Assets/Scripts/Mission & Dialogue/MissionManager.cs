using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MissioneManager : MonoBehaviour
{
    public static MissioneManager Instance;

    public DialogueViewController[] dialogueView;  //拖入Dialogue View Controller的子类；
    public MissionView missionView;                //任务面板，包含大面板和小面板；

    //[HideInInspector]public List<MissionRunTime> currentMission = new();      //存储所有已解锁的任务（包含待接取任务）；
    //[HideInInspector] public List<string> availableMission;   //存储所有可接取的任务；

    [Header("拖入所有的任务")]
    public List<MissionSO> MissionSos;    //存储所有任务的SO；
    public Dictionary<string, MissionRunTime> missionDictionary;    //根据任务ID查询当前任务状态；
    public Dictionary<string, int> conditionDictionary;                  //根据限制ID查询当前限制状态；

    [Header("设置一些相关的信息")]
    public Color fadeColor;
    public float fadeTime;

    [HideInInspector] public bool isInFade = false;

    DialogueSession currentSession;
    bool isConditionChange = false;
    readonly List<DialogueChoose> dailyChoose = new();

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
        conditionDictionary = new Dictionary<string, int>();

        foreach (MissionSO missionSO in MissionSos)
        {
            MissionRunTime missionRun = new()
            {
                missionID = missionSO.missionID,
                //groupIndex = -1,
                groupIndex = 0,
                state = MissionStates.Locked,
                mission = missionSO
            };

            //限制小于1证明没有限制，直接解锁；
            if (missionSO.conditions.Count < 1)
            {
                //判断需不需要额外接取任务，不需要就直接设置为进行中
                if (missionSO.accepted)
                    missionRun.state = MissionStates.Available;
                else
                    missionRun.state = MissionStates.Active;

                //currentMission.Add(missionRun);
            }

            missionDictionary[missionRun.missionID] = missionRun;

        }

    }


    private void OnEnable()
    {
        EventManager.OnAcceptMission += AcceptMission;       //订阅任务解锁事件；
        EventManager.OnConditionChange += ConditionChange;   //订阅限制值改变事件；
    }


    //private void Start()
    //{
    //  运行时从存档内获取进度，并赋值到mission runtime上；

    //}

    private void LateUpdate()
    {
        if (!isConditionChange)
            return;
        if (currentSession != null)
            return;

        isConditionChange = false;
        UpdateMission();

    }


    private void OnDisable()
    {
        EventManager.OnAcceptMission -= AcceptMission;         //取消订阅任务解锁事件；
        EventManager.OnConditionChange -= ConditionChange;     //取消订阅限制值改变事件；
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
        foreach (MissionRunTime run in missionDictionary.Values)
        {
            if (run.TriggerNpcID == NpcID && (run.state == MissionStates.Active || run.state == MissionStates.Available))
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
            currentID = daily.missionID;

            dailyChoose.Clear();

            //遍历所有的已解锁任务，如果有就把这个当作选项；
            foreach(MissionRunTime run in missionDictionary.Values)
            {
                if (run.TriggerNpcID == npcID)
                {
                    DialogueChoose choose = new()
                    {
                        chooseText = run.missionID,
                        chooseType = ChooseTypes.Mission,
                        nextID = run.missionID,
                    };

                    if (run.state == MissionStates.Available)
                    {
                        choose.chooseText = "接取：" + run.missionID;
                        dailyChoose.Add(choose);
                    }
                    else if(run.state == MissionStates.Active)
                    {
                        dailyChoose.Add(choose);
                    }

                }
            }

        }
        else
        {
            //遍历所有已解锁任务，如果有就把第一个当作当前任务；
            foreach (MissionRunTime run in missionDictionary.Values)
            {
                if (run.TriggerNpcID == npcID && (run.state == MissionStates.Active || run.state == MissionStates.Available))
                {
                    currentID = run.missionID;
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
                state = MissionStates.Active,
            };

        ////防止出错，如果当前任务的状态不是已接取，就返回；
        //if (missionRun.state != MissionStates.Active)
        //    return;

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

            //判断当前对话是不是daily对话，且是第一个对话段（即做出第一个选择前）；
            if (currentSession.missionRun.mission.missionType == MissionTypes.Daily && currentSession.paragraphIndex == 0)
            {
                foreach (DialogueChoose choose in dailyChoose)
                {
                    currentChoose.Add(choose);
                }

            }

            //判断对话段有没有选项，有就显示；
            if (currentSession.paragraph.dialogueChooses.Count > 0)
            {
                foreach (DialogueChoose choose in currentSession.paragraph.dialogueChooses)
                {
                    //判断选项是不是跳转到对应的对话组，是就需要判断对应的对话组条件是不是全部解锁，全部解锁才能设为选项；
                    if(choose.chooseType == ChooseTypes.Group)
                    {
                        //遍历该任务所有的对话组，寻找对应的对话组；
                        foreach (DialogueGroupSO group in currentSession.missionRun.mission.groupSOs)
                        {
                            if (group.groupID == choose.chooseText)
                            {
                                //如果对话组有限制，判断是否完成，完成后才能跳转；
                                if (group.conditions.Count > 0)
                                {
                                    if (CanAdvance(group.conditions))
                                    {
                                        currentChoose.Add(choose);
                                    }
                                }
                                //没有限制可直接跳转；
                                else
                                {
                                    currentChoose.Add(choose);
                                }
                                break;
                            }
                        }

                    }
                    else
                    {
                        currentChoose.Add(choose);
                    }

                }

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
            //如果是日常对话，就关闭面板并返回；
            if(currentSession.missionRun.mission.missionType == MissionTypes.Daily)
            {
                currentSession.view.Close();
                currentSession = null;
                return;
            }

            //不是日常对话，调用对话组关闭函数；
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

        if(currentSession.paragraph.effectType == ParagraphOutCome.Default)
        {
            string conditionID = currentSession.missionRun.missionID + "_" + currentSession.missionRun.Group.groupID;

            //更改对应的限制值；
            conditionDictionary[conditionID] = 1;
        }
        else if(currentSession.paragraph.effectType == ParagraphOutCome.Abort)
        {
            //什么都不做，万一后续有操作呢？先留一个位置；
        }

        //无论有没有更改，都置为脏，下一帧更新状态；
        isConditionChange = true;

        if (currentSession.missionRun.groupIndex + 1 >= currentSession.missionRun.mission.groupSOs.Count)       //不会超出索引再加；
        {
            CloseView();
            currentSession.missionRun.state = MissionStates.Complete;

            //更改对应的限制值；
            conditionDictionary[currentSession.missionRun.missionID] = 1;

            currentSession = null;

            await SceneFade.Instance.FadeOut(fadeColor, fadeTime);
            isInFade = false;
            return;
        }

        DialogueGroupSO group = currentSession.missionRun.mission.groupSOs[currentSession.missionRun.groupIndex + 1];

        if (group.isContinuous && (group.conditions.Count < 1 || CanAdvance(group.conditions)))             //如果下一对话组是连续的，并且下一对话组没有任何限制就连续播放下一对话组；
        {
            CloseView();
            currentSession.missionRun.groupIndex++;
            //更新session的数据，开始下一段；
            currentSession.view = GetView(currentSession.group);
            currentSession.paragraphIndex = 0;
            currentSession.lineIndex = 0;

            currentSession.view.OpenView();
            SetLine();
        }
        else
        {
            CloseView();
            currentSession = null;
        }

        await SceneFade.Instance.FadeOut(fadeColor, fadeTime);

        isInFade = false;

    }

    /// <summary>
    /// 调用关闭view面板的函数；
    /// </summary>
    void CloseView()
    {
        currentSession.view.Close();
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
        if (currentSession.group.conditions.Count > 0)
            if (!CanAdvance(currentSession.group.conditions))
            {
                return;
            }

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
            CloseView();
            return;
        }

        currentSession.missionRun = missionRun;
        currentSession.paragraphIndex = 0;
        currentSession.lineIndex = 0;
        currentSession.view = GetView(currentSession.group);

        SetLine();

    }

    /// <summary>
    /// 传入解锁的限制的ID，在字典中获取到对应ID对应的值，并改变对应值；
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="addValue"></param>
    public void ConditionChange(string flag, int addValue)
    {
        if (conditionDictionary.TryGetValue(flag, out int value))
        {
            conditionDictionary[flag] = value + addValue;

            isConditionChange = true;
        }

    }

    /// <summary>
    /// 传入任务ID，代表任务已被接取；
    /// </summary>
    /// <param name="missionID"></param>
    public void AcceptMission(string missionID)
    {
        if (missionDictionary.TryGetValue(missionID, out MissionRunTime missionRun) && missionRun.state == MissionStates.Available)
        {
            missionRun.state = MissionStates.Active;
        }

    }

    /// <summary>
    /// 刷新当前所有任务的状态，判断任务是否可以解锁或推进；
    /// </summary>
    void UpdateMission()
    {
        foreach(MissionRunTime missionRun in missionDictionary.Values)
        {
            //使用分段if且顺序不能变，这样可以在一次遍历中逐个改变；

            //如果当前任务被锁定，判断当前任务能不能被接取；
            if (missionRun.state == MissionStates.Locked)
            {
                //遍历完如果没被置否，就设置为可接取；
                if (CanAdvance(missionRun.mission.conditions))
                {
                    missionRun.state = MissionStates.Available;

                    CanAccept(missionRun);

                }

            }

            //如果当前任务处于待接取状态；
            if(missionRun.state == MissionStates.Available)
            {
                Debug.Log("待接取_" + missionRun.missionID);

                CanAccept(missionRun);

            }

            //如果当前任务正在被追踪，判断当前对话组条件是否满足；
            if(missionRun.state == MissionStates.Active)
            {
                //限制名称用mission ID_groupID；(be like: 初识_main_1)
                string conditionID = missionRun.missionID + "_" + missionRun.Group.groupID;

                Debug.Log("进行中_" + conditionID);

                //如果当前任务的下一对话组的要求全部被满足，就切换至下一对话组；
                if (CanAdvance(conditionID) >= 1 && missionRun.groupIndex + 1 < missionRun.mission.groupSOs.Count)
                {
                    DialogueGroupSO group = missionRun.mission.groupSOs[missionRun.groupIndex + 1];

                    if (group.conditions.Count > 0)
                    {
                        if (CanAdvance(group.conditions))
                        {
                            missionRun.groupIndex++;

                            //预留占位符，写任务面板的更新函数方法；
                        }
                    }
                    else missionRun.groupIndex++;
                }

            }

        }

        missionView.SetSimpleView();

    }

    /// <summary>
    /// 传入限制数组，遍历限制数组并在全部限制解锁时返回true；
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    bool CanAdvance(List<UnlockCondition> conditions)
    {
        foreach (UnlockCondition condition in conditions)
        {
            if (CanAdvance(condition.conditionID) < condition.value)
                return false;
        }

        return true;

    }

    /// <summary>
    /// 传入限制的ID，判断是否存在，并返回值；
    /// </summary>
    /// <param name="conditionID"></param>
    /// <returns></returns>
    public int CanAdvance(string conditionID)
    {
        if (conditionDictionary.TryGetValue(conditionID, out int value))
        {
            return value;
        }
        else return 0;
    }

    /// <summary>
    /// 传入mission Run，判断这个mission Run所对应的任务该不该从待接取状态变为可接取状态；
    /// </summary>
    /// <param name="missionRun"></param>
    void CanAccept(MissionRunTime missionRun)
    {
        if (missionRun.mission.accepted)
        {
            //限制名称用mission ID_groupID；(be like: 初识_main_1)
            string conditionID = missionRun.missionID + "_" + missionRun.mission.groupSOs[0].groupID;

            if (CanAdvance(conditionID) > 0)
            {
                //如果当前接取对话已经完成，就设置为任务状态为正在进行中；
                EventManager.OnTriggerAcceptMission(missionRun.missionID);
            }

        }
        else
        {
            //如果不需要接取，就直接触发
            EventManager.OnTriggerAcceptMission(missionRun.missionID);
        }

        if (missionView.trickMission == null)
        {
            missionView.trickMission = missionRun;
        }

    }


}


/// <summary>
/// 创建任务的运行状态；
/// </summary>
public class MissionRunTime
{
    public string missionID;            //任务ID；
    public int groupIndex;              //当前任务进度；
    public MissionStates state;         //当前任务的状态；

    public MissionSO mission;                                         //当前RunTime所对应的Mission；
    public DialogueGroupSO Group => mission.groupSOs[groupIndex];     //当前任务的对话组；
    public string TriggerNpcID => Group.triggerNpcID;                 //当前任务触发需要的NpcID；
}