using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacteStatView : MonoBehaviour
{
    public GameObject[] views;

    public CharacterView[] characterViews;

    private void OnEnable()
    {
        EventManager.OnDialogue += SetActive;
    }

    private void OnDisable()
    {
        EventManager.OnDialogue -= SetActive;
    }

    void SetActive(bool active)
    {
        foreach(GameObject view in views)
        {
            view.SetActive(active);
        }
    }

}
