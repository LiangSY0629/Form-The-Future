using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public MissionView missionView;

    public MissionRunTime missionRun;

    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(() => missionView.OnPressedMission(missionRun));
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

}
