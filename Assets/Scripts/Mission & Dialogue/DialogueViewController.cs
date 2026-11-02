using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class DialogueViewController : MonoBehaviour
{
    /// <summary>
    /// 将view面板激活；
    /// </summary>
    public abstract void OpenView();


    /// <summary>
    /// 传入需要显示的对话句和切换下一句的函数方法，展示当前句子的文本并在点击下一句时切换下一句；
    /// </summary>
    /// <param name="line"></param>
    /// <param name="OnFinished"></param>
    public abstract void SetLine(DialogueLine line);


    /// <summary>
    /// 传入选项数组和切换对应对话段的函数方法，展示选项，并在点击选项后切换到下一段；
    /// </summary>
    /// <param name="chooses"></param>
    /// <param name="OnChoose"></param>
    public abstract void SetChoose(IReadOnlyList<DialogueChoose> chooses);


    /// <summary>
    /// 关闭当前对话；
    /// </summary>
    public abstract void Close();


    /// <summary>
    /// 按下键盘特定键，切换下一句话；
    /// </summary>
    /// <param name="action"></param>
    public abstract void InputNextLine(InputAction.CallbackContext action);


    /// <summary>
    /// 按下选项，跳转到对应段落并删除所有的选项game object；
    /// </summary>
    /// <param name="paragraphID"></param>
    public abstract void OnPressedChoose(string paragraphID);

}
