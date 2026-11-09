using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MissionView : MonoBehaviour
{
    [Header("设置detail面板")]
    public GameObject detailView;

    public Button[] typeButtons;

    public Transform missionTransform;
    public GameObject missionPrefab;

    public GameObject missionDetail;

    public TMP_Text missionTitle;

    public TMP_Text missionDescription, missionCondition;

    public GameObject missionReward;

    public Button trickButton;

    MissionTypes currentType;
    MissionRunTime currentMission;

    [Header("设置simple面板")]
    public GameObject simpleView;

    public TMP_Text titleText, conditonText;
    public Button detailButton;

    [HideInInspector]public MissionRunTime trickMission; //当前正在追踪的任务；

    private void OnEnable()
    {
        detailButton.onClick.AddListener(() => OnPressedDetail());

        typeButtons[0].onClick.AddListener(() => ChangeType(MissionTypes.Main));
        typeButtons[1].onClick.AddListener(() => ChangeType(MissionTypes.Side));

        trickButton.onClick.AddListener(() => OnTrickMission());
    }

    private void Start()
    {
        //currentMission = MissioneManager.Instance.currentMission[0];
        //初始时加载一下面板，显示当前任务；

        foreach(MissionRunTime missionRun in MissioneManager.Instance.missionDictionary.Values)
        {
            if(missionRun.state == MissionStates.Active)
            {
                trickMission = missionRun;
                break;
            }
        }

        SetSimpleView();

        detailView.SetActive(false);
    }

    private void OnDisable()
    {
        detailButton.onClick.RemoveAllListeners();

        typeButtons[0].onClick.RemoveAllListeners();
        typeButtons[1].onClick.RemoveAllListeners();

        trickButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 设置小面板的显示内容；
    /// </summary>
    public void SetSimpleView()
    {
        if (trickMission != null)
        {
            if(!simpleView.activeSelf)
                simpleView.SetActive(true);

            titleText.text = trickMission.missionID + "：";

            if(trickMission.state == MissionStates.Available)
            {
                conditonText.text = "与" + trickMission.Group.triggerNpcID + "对话接取任务";
            }
            else if(trickMission.state == MissionStates.Active)
            {
                //如果有限制条件
                if(trickMission.Group.conditions.Count > 0)
                {
                    conditonText.text = "";   //初始化未完成限制的文本；
                    int conditionNum = 0;     //记录有多少个限制未完成；

                    foreach(UnlockCondition condition in trickMission.Group.conditions)
                    {
                        if (MissioneManager.Instance.CanAdvance(condition.conditionID) < condition.value)
                        {
                            conditionNum++;

                            //小于三个就显示，大于等于三个就用省略号省略；
                            if (conditionNum < 3)
                                conditonText.text += condition.conditionID + "\n";
                            else if (conditionNum == 3)
                                conditonText.text += "......";
                        }
                    }

                    //如果限制条件全部完成；
                    if (conditionNum < 1)
                    {
                        conditonText.text = "与" + trickMission.Group.triggerNpcID + "对话";
                    }

                }
                //没有限制条件；
                else
                {
                    conditonText.text = "与" + trickMission.Group.triggerNpcID + "对话";
                }

            }
            else if(trickMission.state == MissionStates.Complete)
            {
                titleText.text = "";
                conditonText.text = "";
                simpleView.SetActive(false);
            }

        }
        else
        {
            simpleView.SetActive(false);
        }
    }


    /// <summary>
    /// 设置大面板的内容；
    /// </summary>
    public void SetDetailView()
    {
        List<MissionRunTime> mission = new();

        foreach(Transform tr in missionTransform)
        {
            Destroy(tr.gameObject);
        }

        foreach (MissionRunTime missionRun in MissioneManager.Instance.missionDictionary.Values)
        {
            if(missionRun.state == MissionStates.Active)
            {
                if (missionRun.mission.missionType == currentType)
                    mission.Add(missionRun);
            }
        }

        foreach(MissionRunTime missionRun in mission)
        {
            Mission newMission = Instantiate(missionPrefab, missionTransform).GetComponent<Mission>();
            newMission.missionRun = missionRun;
            newMission.missionView = this;
            newMission.GetComponentInChildren<TMP_Text>().text = missionRun.missionID;
        }

    }

    /// <summary>
    /// 传入类型，将当前任务类型切换为指定类型，并刷新显示；
    /// </summary>
    /// <param name="type"></param>
    public void ChangeType(MissionTypes type)
    {
        currentType = type;
        SetDetailView();
        missionDetail.SetActive(false);
    }


    /// <summary>
    /// 按下小面板的任务详情按钮；
    /// </summary>
    public void OnPressedDetail()
    {
        OpenDetailView();
        if(trickMission == null)
        {
            ChangeType(MissionTypes.Main);
        }
        else
        {
            ChangeType(trickMission.mission.missionType);
            OnPressedMission(trickMission);
        }

    }

    /// <summary>
    /// 打开任务详情面板；
    /// </summary>
    public void OpenDetailView()
    {
        detailView.SetActive(true);
        missionDetail.SetActive(false);
    }

    /// <summary>
    /// 关闭任务详情面板；
    /// </summary>
    public void CloseDetailView()
    {
        detailView.SetActive(false);
    }

    /// <summary>
    /// 检测关闭任务面板的指令；
    /// </summary>
    /// <param name="action"></param>
    public void GetReturnInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            CloseDetailView();
        }
    }

    /// <summary>
    /// 检测打开任务面板的指令
    /// </summary>
    /// <param name="action"></param>
    public void GetMissionInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            OpenDetailView();
            ChangeType(MissionTypes.Main);
        }
    }

    /// <summary>
    /// 点击对应任务，显示详细信息；
    /// </summary>
    public void OnPressedMission(MissionRunTime missionRun)
    {
        if (!missionDetail.activeSelf)
            missionDetail.SetActive(true);

        currentMission = missionRun;

        missionTitle.text = missionRun.missionID;
        missionDescription.text = missionRun.mission.description;

        //如果有限制条件
        if (missionRun.Group.conditions.Count > 0)
        {
            missionCondition.text = "";   //初始化未完成限制的文本；
            int conditionNum = 0;     //记录有多少个限制未完成；

            foreach (UnlockCondition condition in missionRun.Group.conditions)
            {
                if (MissioneManager.Instance.CanAdvance(condition.conditionID) < condition.value)
                {
                    conditionNum++;

                    missionCondition.text += condition.conditionID + "\n";
                }
            }

            //如果限制条件全部完成；
            if (conditionNum < 1)
            {
                conditonText.text = "与" + missionRun.Group.triggerNpcID + "对话";
            }

        }
        //没有限制条件；
        else
        {
            conditonText.text = "与" + missionRun.Group.triggerNpcID + "对话";
        }

        SetTrickButton();

    }

    /// <summary>
    /// 按下追踪任务按钮；
    /// </summary>
    public void OnTrickMission()
    {
        if(currentMission != null)
        {
            trickMission = currentMission;
            SetTrickButton();

            SetSimpleView();
        }
    }

    /// <summary>
    /// 设置追踪按钮的文本；
    /// </summary>
    public void SetTrickButton()
    {
        if (trickMission == currentMission)
        {
            trickButton.GetComponentInChildren<TMP_Text>().text = "追踪中";
        }
        else
        {
            trickButton.GetComponentInChildren<TMP_Text>().text = "追踪任务";
        }
    }

}
