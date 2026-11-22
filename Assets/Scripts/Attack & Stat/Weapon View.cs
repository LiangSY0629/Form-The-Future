using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponView : MonoBehaviour
{
    public GameObject weaponView;
    public Image weaponImage;
    public TMP_Text weaponName;
    public TMP_Text bulletNumber;

    private void LateUpdate()
    {
        if (!HurtManager.Instance.isWeaponChanged && !HurtManager.Instance.isBulletChanged)
            return;

        if(HurtManager.Instance.isWeaponChanged)
        {
            SetWeaponView();
            HurtManager.Instance.isWeaponChanged = false;
        }
        else
        {
            SetBulletText();
            HurtManager.Instance.isBulletChanged = false;
        }
    }


    public void SetWeaponView()
    {
        if (HurtManager.Instance.currentWeapon == null)
        {
            weaponView.SetActive(false);
            return;
        }

        if (!weaponView.activeSelf)
            weaponView.SetActive(true);

        WeaponRunTime weaponRun = HurtManager.Instance.currentWeapon;

        weaponName.text = weaponRun.ID;
        bulletNumber.text = $"{weaponRun.bulletNumber}/{weaponRun.maxBullet}";

        

    }

    void SetBulletText()
    {
        if (HurtManager.Instance.currentWeapon == null)
            return;

        WeaponRunTime weaponRun = HurtManager.Instance.currentWeapon;

        bulletNumber.text = $"{weaponRun.bulletNumber}/{weaponRun.maxBullet}";
    }
}
