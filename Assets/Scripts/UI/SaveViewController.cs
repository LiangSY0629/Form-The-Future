using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SaveViewController : ViewController
{
    public GameObject savePanel;

    public Transform dataTransform;

    public GameObject dataPrefab;

    public TMP_Text tips;

    public Button saveButton;

    public TMP_InputField inputName;

    Archive currentData;

    string currentPath;


    /// <summary>
    /// 显示所有的存档；
    /// </summary>
    public void SetSaveData()
    {
        List<(string, SaveData)> saveDatas = SaveManager.Instance.GetSaveData();

        foreach((string, SaveData) data in saveDatas)
        {
            Archive newData = Instantiate(dataPrefab, dataTransform).GetComponent<Archive>();

            //设置存档的显示信息；
            //newData.saveView = this;
            newData.button.onClick.AddListener(() => OnPressedData(newData));
            newData.texts[0].text = data.Item2.dataName;
            newData.texts[1].text = ToRelative(data.Item2.saveAt);
            newData.path = data.Item1;

        }
    }

    /// <summary>
    /// 删除所有的存档显示
    /// </summary>
    void DestroySaveData()
    {
        foreach(Transform tr in dataTransform)
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
    /// 触发存档；
    /// </summary>
    void SaveGame()
    {
        if (string.IsNullOrWhiteSpace(inputName.text))
        {
            TipsPopController.Instance.SetTips("存档名称不能为空");
            return;
        }

        //如果当前没选择存档进行覆盖 或 要对存档进行重命名，就遍历所有的存档，如果有相同名称的存档就以找到的存档为主；
        if(currentData == null || inputName.text != currentData.texts[0].text)
        {
            FindSameData();
        }
        //没找到就新建一个存档；

        SaveManager.Instance.SaveGame(inputName.text, currentPath);
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


    /// <summary>
    /// 将存档里的时间转换为相对时间；
    /// </summary>
    /// <param name="unixSeconds"></param>
    /// <returns></returns>
    public static string ToRelative(long unixSeconds)
    {
        if (unixSeconds <= 0) return "时间未知";

        TimeSpan span = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

        if (span.TotalMinutes < 1) return "刚刚";
        if (span.TotalHours < 1) return $"{(int)span.TotalMinutes} 分钟前";
        if (span.TotalDays < 1) return $"{(int)span.TotalHours} 小时前";
        if (span.TotalDays < 7) return $"{(int)span.TotalDays} 天前";
        return DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime().ToString("yyyy-MM-dd");
    }

    public override void CloseView()
    {
        inputName.onValueChanged.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();

        DestroySaveData();

        savePanel.SetActive(false);
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
        savePanel.SetActive(true);
        MenuController.Instance.Push(CloseView);

        //监听事件，如果输入框为空不允许保存；
        inputName.onValueChanged.AddListener(text => saveButton.interactable = !string.IsNullOrWhiteSpace(text));

        saveButton.onClick.AddListener(() => SaveGame());
        saveButton.GetComponentInChildren<TMP_Text>().text = "存档";
        tips.text = "输入自定义名称来创建新存档，当名称和旧存档相同时会覆盖旧存档";
        inputName.text = $"存档 {DateTime.Now:MM-dd HH:mm}";

        currentData = null;
        currentPath = null;

        SetSaveData();

    }


}
