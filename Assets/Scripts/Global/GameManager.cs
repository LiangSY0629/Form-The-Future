using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    /// <summary>
    /// 传入mapID，切换到对应的input map，并传入bool值判断要不要显示光标；
    /// </summary>
    /// <param name="mapID"></param>
    /// <param name="needCursor"></param>
    public void SetInputMap(string mapID, bool needCursor)
    {
        playerInput.SwitchCurrentActionMap(mapID);

        if (needCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

}
