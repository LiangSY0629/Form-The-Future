using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public void SaveGame()
    {

        SaveData saveData = new()
        {
            datas = new()
        };

        //遍历所有接口，拼凑json数据；
        foreach (ISaveable saveable in saveables.Values)
        {
            saveData.datas.Add(saveable.GetSaveData());
        }

        File.WriteAllText(save_localtion, JsonUtility.ToJson(saveData));
    }


    /// <summary>
    /// 调用函数加载游戏；
    /// </summary>
    public void LoadGame()
    {
        if (!File.Exists(save_localtion))
        {
            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(save_localtion));

        if (saveData.datas.Count == 0)
            return;

        foreach (Data data in saveData.datas)
        {
            saveables[data.key].SetSaveData(data);
        }
        //加载数据；
    }

}
