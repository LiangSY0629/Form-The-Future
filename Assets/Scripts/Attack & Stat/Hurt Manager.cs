using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HurtManager : MonoBehaviour
{
    public static HurtManager Instance;

    public List<WeaponSO> weaponSos;

    public Dictionary<string, WeaponSO> weaponSoDictionary;
    public Dictionary<int, WeaponRunTime> weaponRunDictionary;

    public WeaponRunTime currentWeapon = null;
    public bool isWeaponChanged;

    public WeaponSocket weaponSocket;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        weaponSoDictionary = new();
        weaponRunDictionary = new();

        foreach (WeaponSO weapon in weaponSos)
        {
            weaponSoDictionary[weapon.ID] = weapon;
        }

        //仅供测试使用，将所有武器全部设置进装备栏
        for (int i = 0; i < weaponSos.Count; i++)
        {
            //Debug.Log("tianjia chenggong" + i);
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
            CharacterManager.Instance.currentCharacter.armStat = ArmStats.Normal;
            weaponSocket.SetNoWeapon();
            currentWeapon = null;
            return;
        }

        if (!weaponRunDictionary.TryGetValue(index, out WeaponRunTime weaponRun))
        {
            //Debug.Log("meizhaodao" + index);
            return;
        }

        if (weaponSocket == null || currentWeapon == weaponRun)
            return;

        CharacterManager.Instance.currentCharacter.armStat = weaponRun.armStat;
        currentWeapon = weaponRun;
        weaponSocket.SetNewWeapon(weaponSoDictionary[weaponRun.ID]);

        isWeaponChanged = true;
        //Debug.Log("已切换");
    }

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


public class WeaponRunTime
{
    public string ID;

    public float attack;

    public int maxBullet;

    public int bulletNumber;

    public ArmStats armStat;
}