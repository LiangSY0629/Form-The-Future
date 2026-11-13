using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttributeViewController : ViewController
{
    public List<Attribute> attributes;

    public Dictionary<string, Attribute> attributeDictionary;

    public GameObject skills;

    public int skillPoint;

    private void Awake()
    {
        attributeDictionary = new Dictionary<string, Attribute>();

        foreach(Attribute attribute in attributes)
        {
            attributeDictionary[attribute.ID] = attribute;
        }

    }

    /// <summary>
    /// 更新当前属性的状态；
    /// </summary>
    void SetAttributes()
    {
        foreach(Attribute attribute in attributeDictionary.Values)
        {
            attribute.SetText();
        }
    }

    /// <summary>
    /// 更新当前技能的状态；
    /// </summary>
    void SetSkills()
    {

    }

    /// <summary>
    /// 传入需要更改的属性，加成类型，加成大小，对目标属性进行更改；
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="addType"></param>
    /// <param name="value"></param>
    public void UpDateAttribute(string ID, AttributeAddTypes addType, float value)
    {
        switch (addType)
        {
            case AttributeAddTypes.Direct: attributeDictionary[ID].directValue += value;break;
            case AttributeAddTypes.Percent: attributeDictionary[ID].percnetValue += value;break;
        }

    }

    public override void OpenView()
    {
        gameObject.SetActive(true);
        MenuController.Instance.Push(CloseView);

        SetAttributes();
        SetSkills();
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

}
