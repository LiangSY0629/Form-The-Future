using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadViewController : ViewController
{
    public GameObject loadPanel;

    public Transform dataTransform;

    public GameObject dataPrefab;

    public TMP_Text tips;

    public Button loadButton;

    public TMP_InputField inputName;

    Archive currentData;

    string currentPath;

    /// <summary>
    /// 显示所有的存档；
    /// </summary>
    public void SetSaveData()
    {
        List<(string, SaveData)> saveDatas = SaveManager.Instance.GetSaveData();

        foreach ((string, SaveData) data in saveDatas)
        {
            Archive newData = Instantiate(dataPrefab, dataTransform).GetComponent<Archive>();

            //设置存档的显示信息；
            //newData.saveView = this;
            newData.button.onClick.AddListener(() => OnPressedData(newData));
            newData.texts[0].text = data.Item2.dataName;
            newData.texts[1].text = SaveViewController.ToRelative(data.Item2.saveAt);
            newData.path = data.Item1;

        }
    }


    /// <summary>
    /// 删除所有的存档显示
    /// </summary>
    void DestroySaveData()
    {
        foreach (Transform tr in dataTransform)
        {
            Destroy(tr.gameObject);
        }
    }

    /// <summary>
    /// 传入按下的那个存档；
    /// </summary>
    /// <param name="archive"></param>
    public void OnPressedData(Archive archive)
    {
        currentPath = archive.path;
        inputName.text = archive.texts[0].text;
    }


    /// <summary>
    /// 加载存档；
    /// </summary>
    async void LoadGame()
    {
        //如果当前选择的存档名称和输入框的名称不同，优先以输入的名称为主；
        if (currentData == null || inputName.text != currentData.texts[0].text)
        {
            FindSameData();

            if (string.IsNullOrWhiteSpace(currentPath))
            {
                TipsPopController.Instance.SetTips("没找到对应存档");
                return;
            }

        }
        //没找到也不操作，不报错，只是以当前选择为主；

        if (string.IsNullOrWhiteSpace(currentPath))
        {
            TipsPopController.Instance.SetTips("请选择一个存档");
            return;
        }

        await SceneFade.Instance.FadeIn(Color.black, 1);

        SaveManager.Instance.LoadGame(currentPath);

        await SceneFade.Instance.FadeOut(Color.black, 1);
    }

    /// <summary>
    /// 遍历所有存档，查找当前输入框的名称，找到就更改当前存档路径为目标路径；
    /// </summary>
    void FindSameData()
    {
        foreach (Transform tr in dataTransform)
        {
            Archive data = tr.GetComponent<Archive>();
            if (data.texts[0].text == inputName.text)
            {
                currentPath = data.path;
                break;
            }
        }
    }


    public override void CloseView()
    {
        //inputName.onValueChanged.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();

        DestroySaveData();

        loadPanel.SetActive(false);

    }

    public override void GetOpenInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OpenView();
        }
    }

    public override void OpenView()
    {
        loadPanel.SetActive(true);
        MenuController.Instance.Push(CloseView);

        //监听事件，如果输入框为空不允许保存；
        //inputName.onValueChanged.AddListener(text => loadButton.interactable = !string.IsNullOrWhiteSpace(text));

        loadButton.onClick.AddListener(() => LoadGame());
        loadButton.GetComponentInChildren<TMP_Text>().text = "加载";
        tips.text = "点击选择一个存档或输入存档名称进行加载";
        inputName.text = null;

        currentData = null;
        currentPath = null;

        SetSaveData();
    }

}
