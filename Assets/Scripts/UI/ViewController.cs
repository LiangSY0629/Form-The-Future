using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ViewController : MonoBehaviour
{
    public string viewID;

    /// <summary>
    /// 打开面板；
    /// </summary>
    public abstract void OpenView();

    /// <summary>
    /// 关闭面板；
    /// </summary>
    public abstract void CloseView();

    /// <summary>
    /// 通过按键打开面板；
    /// </summary>
    /// <param name="ctx"></param>
    public abstract void GetOpenInput(InputAction.CallbackContext ctx);

}
