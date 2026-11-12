using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Archive : MonoBehaviour
{
    public TMP_Text[] texts;

    public Button button;

    public string path;

    void Awake()
    {
        texts = GetComponentsInChildren<TMP_Text>();
        button = GetComponent<Button>();
    }


    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

}
