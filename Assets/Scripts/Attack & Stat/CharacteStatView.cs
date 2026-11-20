using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacteStatView : MonoBehaviour
{
    public Button[] armButtons;

    public CharacterView[] characterViews;

    private void OnEnable()
    {
        armButtons[0].onClick.AddListener(() => CharacterManager.Instance.SwitchArmStat(0));

        armButtons[1].onClick.AddListener(() => CharacterManager.Instance.SwitchArmStat(1));
    }

    private void OnDisable()
    {
        armButtons[0].onClick.RemoveAllListeners();
        armButtons[1].onClick.RemoveAllListeners();
    }

}
