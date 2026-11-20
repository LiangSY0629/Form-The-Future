using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public List<CharacterSO> characterSos;

    public List<ArmStats> armStats;

    public Dictionary<string, CharacterRunTime> characterSoDictionary;

    public CharacterRunTime currentCharacter;

    Dictionary<string, float> affinityDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        affinityDictionary = new();
        characterSoDictionary = new();

        foreach (CharacterSO character in characterSos)
        {
            CharacterRunTime characterRun = new()
            {
                ID = character.charaterID,
                characterSo = character,
                armStat = ArmStats.Normal,
            };

            characterSoDictionary[character.charaterID] = characterRun;

            affinityDictionary[character.charaterID] = 0;
        }

        if(currentCharacter == null)
        {
            currentCharacter = characterSoDictionary[characterSos[0].charaterID];
        }

    }

    /// <summary>
    /// 切换当前角色的武器状态；
    /// </summary>
    /// <param name="armStat"></param>
    public void SwitchArmStat(int index)
    {
        if (index > armStats.Count)
            return;

        currentCharacter.armStat = armStats[index];
        Debug.Log("已切换");
    }

    /// <summary>
    /// 传入ID获取对应角色好感度
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public float GetAffinityValue(string ID)
    {
        affinityDictionary.TryGetValue(ID, out float value);

        return value;
    }

    /// <summary>
    /// 传入ID和值，让当前好感度提升对应值；
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public void SetAffinityValue(string ID, float value) => affinityDictionary[ID] += value;


    /// <summary>
    /// 检测当前键盘输入的数字，判断切换第几个武器；
    /// </summary>
    /// <param name="ctx"></param>
    public void GetArmInput(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        string key = ctx.control.name;

        if (int.TryParse(key, out int slot))
            SwitchArmStat(slot - 1);

    }

}


public class CharacterRunTime
{
    public string ID;

    public CharacterSO characterSo;

    public ArmStats armStat;

    public Dictionary<string, float> cooldownDictionary;

    public List<Stat> currentStats;
}