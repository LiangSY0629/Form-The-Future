using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    public TMP_Text missionTitle;

    public TMP_Text missionDescription, missionCondition;

    public GameObject missionReward;

    public Button trickButton;

    MissionTypes currentType;

    [Header("设置simple面板")]
    public GameObject simpleView;

    public TMP_Text titleText, conditonText;
    public Button detailButton;

    [HideInInspector]public MissionRunTime currentMission; //当前正在追踪的任务；

    private void OnEnable()
    {
        detailButton.onClick.AddListener(() => OpenDetailView());

        typeButtons[0].onClick.AddListener(() => ChangeType(MissionTypes.Main));
        typeButtons[1].onClick.AddListener(() => ChangeType(MissionTypes.Side)); 
    }

    private void Start()
    {
        //currentMission = MissioneManager.Instance.currentMission[0];
        //初始时加载一下面板，显示当前任务；

        foreach(MissionRunTime missionRun in MissioneManager.Instance.missionDictionary.Values)
        {
            if(missionRun.state == MissionStates.Active)
            {
                currentMission = missionRun;
                break;
            }
        }

        SetSimpleView();
    }

    private void OnDisable()
    {
        detailButton.onClick.RemoveAllListeners();

        typeButtons[0].onClick.RemoveAllListeners();
        typeButtons[1].onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 设置小面板的显示内容；
    /// </summary>
    public void SetSimpleView()
    {
        if (currentMission != null)
        {
            if(!simpleView.activeSelf)
                simpleView.SetActive(true);

            titleText.text = currentMission.missionID + "：";

            if(currentMission.state == MissionStates.Available)
            {
                conditonText.text = "与" + currentMission.Group.triggerNpcID + "对话接取任务";
            }
            else if(currentMission.state == MissionStates.Active)
            {
                //如果有限制条件
                if(currentMission.Group.conditions.Count > 0)
                {
                    conditonText.text = "";   //初始化未完成限制的文本；
                    int conditionNum = 0;     //记录有多少个限制未完成；

                    foreach(UnlockCondition condition in currentMission.Group.conditions)
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
                        conditonText.text = "与" + currentMission.Group.triggerNpcID + "对话";
                    }

                }
                //没有限制条件；
                else
                {
                    conditonText.text = "与" + currentMission.Group.triggerNpcID + "对话";
                }

            }
            else if(currentMission.state == MissionStates.Complete)
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
            GameObject newMission = Instantiate(missionPrefab, missionTransform);
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
    }

    /// <summary>
    /// 打开任务详情面板；
    /// </summary>
    public void OpenDetailView()
    {
        currentType = MissionTypes.Main;
        SetDetailView();
        detailView.SetActive(true);
    }

    /// <summary>
    /// 关闭任务详情面板；
    /// </summary>
    public void CloseDetailView()
    {
        detailView.SetActive(false);
    }

    public void GetReturnInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            CloseDetailView();
        }
    }

    public void GetMissionInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            OpenDetailView();
        }
    }

}
