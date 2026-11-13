using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    public GameObject mainMenuView;

    public GameObject buttonPrefab;

    public MissionView missionView;

    public List<ViewController> views;

    public Dictionary<string, ViewController> viewDictionary;

    public Transform buttonTransform;

    /// <summary>
    /// 通常当作栈来使用，为后续可能情况预留空间；
    /// </summary>
    public List<Action> openMenu = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        viewDictionary = new Dictionary<string, ViewController>();
    }

    private void Start()
    {
        foreach(ViewController view in views)
        {
            viewDictionary[view.viewID] = view;

            Button button = Instantiate(buttonPrefab, buttonTransform).GetComponent<Button>();

            button.onClick.AddListener(() => OpenView(view.viewID));

            button.GetComponentInChildren<TMP_Text>().text = view.viewID;
        }
    }

    private void OnDestroy()
    {
        foreach(Transform tr in buttonTransform)
        {
            tr.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(tr.gameObject);
        }
    }


    /// <summary>
    /// 将传入的方法放入list中，调用时按顺序调用；
    /// </summary>
    /// <param name="action"></param>
    public void Push(Action action)
    {
        //加入时如果没有一个打开，就切换为UI；
        if (openMenu.Count == 0)
        {
            GameManager.Instance.SetInputMap("UI", true);
        }

        openMenu.Add(action);

        if (action != CloseMenuView && openMenu.Contains(CloseMenuView))
        {
            CloseMenuView();
            openMenu.Remove(CloseMenuView);
        }

    }

    /// <summary>
    /// 从list的尾部取出方法并调用；
    /// </summary>
    public void Pop()
    {
        if (openMenu.Count == 0)
            return;

        int index = openMenu.Count - 1;

        Action close = openMenu[index];
        openMenu.RemoveAt(index);

        close();

        //移除后如果没有任何需要关闭的，就切换为移动；
        if (openMenu.Count == 0)
            GameManager.Instance.SetInputMap("Player", false);
    }

    /// <summary>
    /// 检测esc输入；
    /// </summary>
    /// <param name="ctx"></param>
    public void GetOpenInput(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;


        if (openMenu.Count > 0)
        {
            Pop();
            return;
        }

        OpenMenuView();

    }

    /// <summary>
    /// 传入需要调用的方法，如果能找到就调用，找不到自动调用最后添加的面板；
    /// </summary>
    /// <param name="action"></param>
    public void Pop(Action action)
    {
        if (openMenu.Count == 0)
            return;

        if (openMenu.Contains(action))
        {
            action();
            openMenu.Remove(action);
        }
        else
        {
            action();
        }

        //移除后如果没有任何需要关闭的，就切换为移动；
        if (openMenu.Count == 0)
            GameManager.Instance.SetInputMap("Player", false);
    }

    /// <summary>
    /// 传入需要调用的方法，如果有该方法，将调用这个方法之后的所有方法；
    /// </summary>
    /// <param name="action"></param>
    public void Pops(Action action)
    {
        if (openMenu.Count == 0)
            return;

        int index = -1;

        for(int i = 0; i < openMenu.Count; i++)
        {
            if (action == openMenu[i])
            {
                index = i;
                break;
            }
        }

        if (index >= 0)
        {
            for (int i = index; i < openMenu.Count; i++)
            {
                openMenu[i]();
            }

            openMenu.RemoveRange(index, openMenu.Count - index);

        }

        //移除后如果没有任何需要关闭的，就切换为移动；
        if (openMenu.Count == 0)
            GameManager.Instance.SetInputMap("Player", false);

    }


    public void OpenMenuView()
    {
        mainMenuView.SetActive(true);
        Push(CloseMenuView);
    }

    public void CloseMenuView()
    {
        mainMenuView.SetActive(false);
    }

    public void OpenMissionView()
    {
        missionView.OpenDetailView();
        missionView.ChangeType(MissionTypes.Main);
    }

    /// <summary>
    /// 传入参数打开对应面板；
    /// </summary>
    /// <param name="index"></param>
    public void OpenView(string key)
    {
        if (!viewDictionary.TryGetValue(key, out ViewController view))
            return;

        view.OpenView();

    }

}
