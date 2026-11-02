using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class MissioneManager : MonoBehaviour
{
    public static MissioneManager Instance;

    public DialogueViewController[] dialogueView;  //拖入Dialogue View Controller的子类；

    public List<string> currentMission;   //存储所有已解锁的任务；

    public List<MissionSO> MissionSos;    //存储所有任务的SO；

    public Dictionary<string, MissionRunTime> missionDictionary;    //根据任务ID查询当前任务状态；

    DialogueSession currentSession;

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
        public DialogueGroupSO group;
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
    /// 传入当前的NpcID显示对应任务的对话，
    /// </summary>
    /// <param name="NpcID"></param>
    public void SetDialogue(string NpcID)
    {
        string currentID = "";

        foreach (string ID in currentMission)
        {
            if (missionDictionary[ID].TriggerNpcID == NpcID)       //从前往后找，找到匹配的就终止，后续改为用list全部显示；
            {
                currentID = ID;
                break;
            }

        }

        //判断是否可以正常获取到Runtime，防止出错；
        if (!missionDictionary.TryGetValue(currentID, out MissionRunTime missionRun))
            return;

        DialogueViewController view = GetView(missionRun.Group);

        DialogueSession session = new()
        {
            view = view,
            missionRun = missionRun,
            group = missionRun.Group,
            paragraphIndex = 0,
            lineIndex = 0,

        };

        currentSession = session;

        session.view.OpenView();
        session.view.SetLine(session.line);

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
    /// 切换下一句，如果没有下一句，显示选项，如果没有下一句也没有选项，关闭对话；
    /// </summary>
    public void NextLine()
    {

        if(currentSession.lineIndex + 1 < currentSession.paragraph.dialogueLines.Count)                     //判断有没有下一句；
        {
            currentSession.lineIndex++;
            currentSession.view.SetLine(currentSession.line);
        }
        else                                                                                              //没有就准备切换对话段；
        {
            if (currentSession.paragraph.dialogueChooses.Count > 0)                                       //判断有没有选项，有就显示选项；
            {
                IReadOnlyList<DialogueChoose> chooses = currentSession.paragraph.dialogueChooses;
                currentSession.view.SetChoose(chooses);
            }
            else
            {

                if(currentSession.missionRun.groupIndex + 1 < currentSession.missionRun.mission.groupSOs.Count)       //不会超出索引再加；
                {
                    currentSession.missionRun.groupIndex++;
                }
                else
                {
                    currentSession.view.Close();
                    currentSession.missionRun.complete = true;
                    currentMission.Remove(currentSession.missionRun.missionID);

                    currentSession = null;
                    return;
                }

                if (currentSession.group.isContinuous)             //如果当前对话组是连续的，就连续播放下一对话组；
                {
                    currentSession.view.Close();

                    //更新session的数据，开始下一段；
                    currentSession.view = GetView(currentSession.group);
                    currentSession.group = currentSession.missionRun.Group;
                    currentSession.paragraphIndex = 0;
                    currentSession.lineIndex = 0;

                    currentSession.view.OpenView();
                    currentSession.view.SetLine(currentSession.line);
                    return;
                }
                else
                {
                    currentSession.view.Close();
                    currentSession = null;
                    return;
                }

            }
                

        }


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

        currentSession.view.SetLine(currentSession.line);

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
        foreach(string ID in currentMission)
        {
            if (missionDictionary[ID].TriggerNpcID == NpcID)
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
        missionDictionary[missionID].groupIndex = 0;
    }


}
