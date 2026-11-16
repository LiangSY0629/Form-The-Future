using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatViewController : ViewController
{
    public GameObject statPrefab;

    public Button characterButton;

    public List<StatView> statViews;

    //public Dictionary<StatTypes, StatView> statViewDictionary;

    public SkillView[] skills;

    public TMP_Text skillTitle, skillDescription, skillLock;

    public string currentCharacterID;


    private void Awake()
    {
        foreach(SkillView skill in skills)
        {
            skill.statViewController = this;
        }

    }

    private void OnEnable()
    {
        characterButton.onClick.AddListener(() => SetDescriptionView());
        EventManager.OnStatChanged += SetStatView;
    }

    private void OnDisable()
    {
        characterButton.onClick.RemoveAllListeners();
        EventManager.OnStatChanged -= SetStatView;
    }

    /// <summary>
    /// 刷新所有属性显示；
    /// </summary>
    public void SetStatView()
    {
        foreach(StatView statView in statViews)
        {
            if(StatManager.Instance.statRunDictionary.TryGetValue(statView.type, out StatRunTime statRun))
            {
                statView.SetValue(statRun.baseValue, statRun.finalValue);
            }
        }
    }

    /// <summary>
    /// 按下技能按钮显示详情；
    /// </summary>
    /// <param name="skill"></param>
    public void SetDescriptionView(SkillView skill)
    {
        if (skill.skillSo == null)
            return;

        skillTitle.text = skill.ID;
        skillDescription.text = skill.decription;

        if (CharacterManager.Instance.GetAffinityValue(currentCharacterID) < skill.lockValue)
            skillLock.text = $"好感度达到{skill.lockValue}后解锁";
        else skillLock.text = "已解锁";
    }

    /// <summary>
    /// 初始化详情描述；
    /// </summary>
    void SetDescriptionView()
    {
        if (!CharacterManager.Instance.characterSoDictionary.TryGetValue(currentCharacterID, out CharacterSO character))
            return;

        skillTitle.text = character.charaterID;
        skillDescription.text = character.description;
        skillLock.text = "";
    }

    /// <summary>
    /// 显示角色及信息；
    /// </summary>
    public void SetCharacterView()
    {
        //如果当前有角色，就显示当前角色的属性，否则显示第一个角色的状态；
        if (CharacterManager.Instance.currentCharacter != null)
            currentCharacterID = CharacterManager.Instance.currentCharacter.ID;
        else currentCharacterID = CharacterManager.Instance.characterSos[0].charaterID;

        SetSkillView();
        SetStatView();
    }

    /// <summary>
    /// 更新技能图标的显示；
    /// </summary>
    public void SetSkillView()
    {
        if (!CharacterManager.Instance.characterSoDictionary.TryGetValue(currentCharacterID, out CharacterSO character))
            return;

        for (int i = 0; i < skills.Length; i++)
        {
            SkillView skill = skills[i];

            if (i >= character.skills.Count)
            {
                skill.gameObject.SetActive(false);
            }
            else
            {
                skill.skillSo = character.skills[i];

                //判断是否达到解锁条件，没达到置为灰色，达到恢复原来的颜色；
                if (CharacterManager.Instance.GetAffinityValue(currentCharacterID) < skill.lockValue)
                    skill.skillImage.color = 0.5f * Color.gray;
                else skill.skillImage.color = Color.white;
            }
        }

        SetDescriptionView();

    }

    public void SwitchCharacter(int value)
    {

    }

    #region open&close

    public override void OpenView()
    {
        gameObject.SetActive(true);
        MenuController.Instance.Push(CloseView);

        SetCharacterView();
    }

    public override void GetOpenInput(InputAction.CallbackContext cxt)
    {
        if (cxt.performed)
        {
            OpenView();
        }
    }

    public override void CloseView()
    {
        gameObject.SetActive(false);
    }

    public void OnPressedClose()
    {
        MenuController.Instance.Pop(CloseView);
    }

    #endregion
}
