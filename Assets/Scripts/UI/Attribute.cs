using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Attribute : MonoBehaviour
{
    public string ID;

    public float baseValue;

    ///最终值等于基础值乘以百分比加成再加上直接加成；
    public float FinalValue => (baseValue * (1 + percnetValue)) + directValue;

    public float directValue;

    public float percnetValue = 1;

    public TMP_Text IDText;

    public TMP_Text text;

    public Button addButton;

    private void Awake()
    {
        IDText.text = ID;
    }

    public void SetText() 
    {
        text.text = $"{baseValue}（+{baseValue * percnetValue + directValue}）";
    }


}
