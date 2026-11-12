using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public string save_localtion;

    public Dictionary<string, ISaveable> saveables = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        save_localtion = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    /// <summary>
    /// 调用函数保存游戏；
    /// </summary>
    public void SaveGame(string dataName, string savePath = null)
    {

        SaveData saveData = new()
        {
            //version = 1,
            dataName = dataName,
            saveAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            datas = new()
        };

        savePath ??= Path.Combine(GetSaveDir(), $"save_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        //遍历所有接口，拼凑json数据；
        foreach (ISaveable saveable in saveables.Values)
        {
            saveData.datas.Add(saveable.GetSaveData());
        }

        File.WriteAllText(savePath, JsonUtility.ToJson(saveData, true));

        TipsPopController.Instance.SetTips("存档成功！");

        //此时存档是最后一个打开的界面，所以直接关闭最后一个即可；
        MenuController.Instance.Pop();
    }

    /// <summary>
    /// 获取存档地址；
    /// </summary>
    /// <returns></returns>
    string GetSaveDir()
    {
        string dir = Path.Combine(Application.persistentDataPath, "saves");
        Directory.CreateDirectory(dir);
        return dir;
    }


    /// <summary>
    /// 调用函数加载游戏；
    /// </summary>
    public void LoadGame(string loadPath)
    {
        if (loadPath == null)
        {
            TipsPopController.Instance.SetTips("请选择一个存档");
            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(loadPath));

        if (saveData == null || saveData.datas == null || saveData.datas.Count == 0)
        {
            TipsPopController.Instance.SetTips("当前存档为空存档或存档已损坏！");
            return;
        }

        foreach (Data data in saveData.datas)
        {
            if (saveables.TryGetValue(data.key, out ISaveable saveable))
                saveable.SetSaveData(data);
        }

        TipsPopController.Instance.SetTips("加载成功!");

        //此时加载是最后一个打开的界面，所以直接关闭最后一个即可；
        MenuController.Instance.Pop();
    }


    /// <summary>
    /// 获取所有的存档；
    /// </summary>
    /// <returns></returns>
    public List<(string path, SaveData data)> GetSaveData()
    {
        List<(string, SaveData)> datas = new();

        foreach (string file in Directory.GetFiles(GetSaveDir(), "*.json"))
        {
            try
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(file));
                if (saveData != null) datas.Add((file, saveData));
            }
            catch
            {
                Debug.Log("跳过已损坏的存档" + file);
            }
        }

        //重新排序，比较存储时时间大小，新存的在前面；
        datas.Sort((a, b) => b.Item2.saveAt.CompareTo(a.Item2.saveAt));

        return datas;

    }

}
