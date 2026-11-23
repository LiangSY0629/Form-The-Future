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

    public bool isWeaponChanged = false;
    public bool isBulletChanged = false;
    public bool isReloading = false;

    public bool isBulletEmpty = false;
    public bool isFire = false;

    public WeaponSocket weaponSocket;

    float nextFireTime = 0;

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
                fireInterval = (float)60 / weapon.fireRate,
                maxBullet = weapon.maxBullet,
                armStat = weapon.armStat,
                bulletNumber = weapon.maxBullet,
            };

            weaponRunDictionary[i + 2] = weaponRun;

        }

    }

    private void Update()
    {
        if (isFire)
        {
            if (nextFireTime <= 0)
                WeaponFire();
            else nextFireTime -= Time.deltaTime;
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
    /// 开始换弹，检测是否需要，不需要换弹就返回false，不播放换弹动画，反之播放换弹；
    /// </summary>
    /// <returns></returns>
    public bool StartReload()
    {
        if (currentWeapon.bulletNumber == currentWeapon.maxBullet)
        {
            TipsPopController.Instance.SetTips("当前不需要换弹");
            return false;
        }

        isReloading = true;
        return true;
    }

    /// <summary>
    /// 设置开火状态，并归零下一次开火的间隔；
    /// </summary>
    /// <param name="fireing"></param>
    public void SetFire(bool fireing)
    {
        isFire = fireing;
        nextFireTime = 0;
    }


    /// <summary>
    /// 武器开火
    /// </summary>
    public void WeaponFire()
    {
        currentWeapon.bulletNumber--;
        nextFireTime = currentWeapon.fireInterval;

        if (currentWeapon.bulletNumber < 1)
        {
            isBulletEmpty = true;
            SetFire(false);
        }

        //Debug.Log(nextFireTime);
        isBulletChanged = true;
    }

    /// <summary>
    /// 换弹自然停止，剩余弹药数量更新；
    /// </summary>
    public void EndReload()
    {
        isReloading = false;
        currentWeapon.bulletNumber = currentWeapon.maxBullet;
        isBulletChanged = true;
        isBulletEmpty = false;
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

    /// <summary>
    /// 开火之间的间隔；
    /// </summary>
    public float fireInterval;

    public int maxBullet;

    public int bulletNumber;

    public ArmStats armStat;
}