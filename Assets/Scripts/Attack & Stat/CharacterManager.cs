using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public List<CharacterSO> characterSos;

    public List<WeaponSO> weaponSos;

    public Dictionary<string, CharacterRunTime> characterSoDictionary;

    public Dictionary<string, WeaponSO> weaponSoDictionary;
    public Dictionary<int, WeaponRunTime> weaponRunDictionary;

    public CharacterRunTime currentCharacter;

    Dictionary<string, float> affinityDictionary;
    public WeaponSocket weaponSocket;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        affinityDictionary = new();
        characterSoDictionary = new();
        weaponSoDictionary = new();
        weaponRunDictionary = new();

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

        currentCharacter ??= characterSoDictionary[characterSos[0].charaterID];

        foreach(WeaponSO weapon in weaponSos)
        {
            weaponSoDictionary[weapon.ID] = weapon;
        }

        //仅供测试使用，将所有武器全部设置进装备栏
        for(int i = 0; i < weaponSos.Count; i++)
        {
            Debug.Log("tianjia chenggong" + i);
            WeaponSO weapon = weaponSos[i];

            WeaponRunTime weaponRun = new()
            {
                ID = weapon.ID,
                attack = weapon.attack,
                maxBullet = weapon.maxBullet,
                armStat = weapon.armStat,
                bulletNumber = weapon.maxBullet,
            };

            weaponRunDictionary[i + 2] = weaponRun;

        }

    }

    /// <summary>
    /// 切换当前角色的武器状态；
    /// </summary>
    /// <param name="index"></param>
    public void SwitchArmStat(int index)
    {
        if (index == 1)
        {
            currentCharacter.armStat = ArmStats.Normal;
            weaponSocket.SetNoWeapon();
            return;
        }

        if (!weaponRunDictionary.TryGetValue(index, out WeaponRunTime weaponRun))
        {
            Debug.Log("meizhaodao" + index);
            return;
        }

        if (weaponSocket == null)
            return;

        currentCharacter.armStat = weaponRun.armStat;
        weaponSocket.SetNewWeapon(weaponSoDictionary[weaponRun.ID]);

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
            SwitchArmStat(slot);

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

public class WeaponRunTime
{
    public string ID;

    public float attack;

    public int maxBullet;

    public int bulletNumber;

    public ArmStats armStat;
}