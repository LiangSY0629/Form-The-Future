using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{
    public Button skillButton;

    public TMP_Text skillName;

    public Image skillImage;

    public SkillSO skillSo;

    public string decription => skillSo.description;

    public string ID => skillSo.skillID;

    public float lockValue => skillSo.lockValue;

    public StatViewController statViewController;

    private void OnEnable()
    {
        skillButton.onClick.AddListener(() => statViewController.SetDescriptionView(this));
    }

    private void OnDisable()
    {
        skillButton.onClick.RemoveAllListeners();   
    }

}
