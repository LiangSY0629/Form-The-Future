using UnityEngine;

public class WeaponSocket : MonoBehaviour
{
    public WeaponSO currentWeapon;

    public Transform weaponTransform;

    GameObject weapon;

    /// <summary>
    /// 生成新的武器；
    /// </summary>
    /// <param name="weaponSo"></param>
    public void SetNewWeapon(WeaponSO weaponSo)
    {
        if (weaponTransform == null
            || weaponSo == null
            || (currentWeapon == weaponSo && weapon != null)
            || weaponSo.prefab == null)
            return;


        if (weapon != null)
            Destroy(weapon);

        currentWeapon = weaponSo;
        weapon = Instantiate(weaponSo.prefab, weaponTransform);

    }


    /// <summary>
    /// 设置为空手状态；
    /// </summary>
    public void SetNoWeapon()
    {
        if (weapon == null)
            return;

        Destroy(weapon);
        currentWeapon = null;

    }
}