using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatViewController : ViewController
{
    public Dictionary<string, StatView> statViewDicionary = new();

    public GameObject statPrefab;

    public Transform statTransform;

    private void Start()
    {
        foreach(Stat stat in StatManager.Instance.stats)
        {
            StatView statView = Instantiate(statPrefab, statTransform).GetComponent<StatView>();

            statView.SetAllText(stat.ID, stat.value, stat.addOnUp);
            statViewDicionary[stat.ID] = statView;
        }

        SetStatView();
    }

    private void OnEnable()
    {
        SetStatView();
    }

    /// <summary>
    /// 刷新所有属性显示；
    /// </summary>
    void SetStatView()
    {
        foreach(StatView stat in statViewDicionary.Values)
        {
            if (StatManager.Instance.statDictionary.TryGetValue(stat.statID, out StatRunTime statRun))
                stat.SetValue(statRun.baseValue, statRun.finalValue);
        }
    }
    
    /// <summary>
    /// 刷新指定的属性显示；
    /// </summary>
    /// <param name="ID"></param>
    public void SetStatView(string ID)
    {
        if(statViewDicionary.TryGetValue(ID, out StatView statView) && StatManager.Instance.statDictionary.TryGetValue(ID, out StatRunTime statRun))
        {
            statView.SetValue(statRun.baseValue, statRun.finalValue);
        }
    }

    public override void OpenView()
    {
        gameObject.SetActive(true);
        MenuController.Instance.Push(CloseView);
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

}
