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
        if (!HurtManager.Instance.isWeaponChanged)
            return;

        SetWeaponView();

        HurtManager.Instance.isWeaponChanged = false;
    }


    public void SetWeaponView()
    {
        if (HurtManager.Instance.currentWeapon == null)
        {
            weaponView.SetActive(false);
            return;
        }
        else
        {
            if (!weaponView.activeSelf)
                weaponView.SetActive(true);

            WeaponRunTime weaponRun = HurtManager.Instance.currentWeapon;

            weaponName.text = weaponRun.ID;
            bulletNumber.text = $"{weaponRun.bulletNumber}/{weaponRun.maxBullet}";

        }

    }
}
