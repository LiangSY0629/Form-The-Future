using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatView : MonoBehaviour
{
    public string ID;

    public StatTypes type;

    public TMP_Text titleText, valueText;

    public float baseModifier;

    private void Awake()
    {
        titleText.text = ID;
    }
    
    public void SetValue(float baseValue, float finalValue)
    {
        valueText.text = $"{baseValue}£¨+{finalValue - baseValue}£©";
    }

}
