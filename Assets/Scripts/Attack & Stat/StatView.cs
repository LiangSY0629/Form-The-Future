using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatView : MonoBehaviour
{
    public string statID;

    public TMP_Text titleText, valueText;

    public Button button;

    public float addOnUp;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();

        titleText = texts[0];
        valueText = texts[1];
    }

    private void OnEnable()
    {
        button.onClick.AddListener(() => StatManager.Instance.StatBaseModifier(statID, addOnUp));
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetAllText(string ID, float baseValue, float value)
    {
        statID = ID;
        titleText.text = ID;

        addOnUp = value;
    }

    public void SetValue(float baseValue, float finalValue)
    {
        valueText.text = $"{baseValue}£¨+{finalValue - baseValue}£©";
    }



}
