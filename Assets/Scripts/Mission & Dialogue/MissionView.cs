using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionView : MonoBehaviour
{
    public GameObject detailView;

    public GameObject simpleView;

    public TMP_Text titleText, conditonText;
    public Button detailButton;

    public MissionRunTime currentMission;

    private void Start()
    {
        currentMission = MissioneManager.Instance.missionDictionary[MissioneManager.Instance.currentMission[0]];
        //初始时加载一下面板，显示当前任务；
        SetSimpleView();
    }

    public void SetSimpleView()
    {
        if (currentMission != null)
        {
            if(!simpleView.activeSelf)
                simpleView.SetActive(true);

            titleText.text = currentMission.missionID;
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

}
