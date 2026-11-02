using System;


public interface IInteractable
{
    /// <summary>
    /// 能不能进行交互；
    /// </summary>
    /// <returns></returns>
    bool CanInteracte();

    /// <summary>
    /// 进行交互；
    /// </summary>
    void Interactable();

}


public interface IDialogueNPC
{
    /// <summary>
    /// 获取NPC的ID；
    /// </summary>
    /// <returns></returns>
    string DialogueNPC();

    /// <summary>
    /// 判断当前npc能不能对话；
    /// </summary>
    /// <returns></returns>
    bool CanDialogue();

}


//public interface IDialogueView
//{
//    /// <summary>
//    /// 传入当前对话句，调用action切换下一句；
//    /// </summary>
//    /// <param name="line"></param>
//    /// <param name="OnFinished"></param>
//    void SetLine(DialogueLine line, Action OnFinished);

//    /// <summary>
//    /// 传入选项，调用action切换到对应段；
//    /// </summary>
//    /// <param name="choose"></param>
//    /// <param name="OnChoose"></param>
//    void SetChoose(DialogueChoose choose, Action OnChoose);

//    /// <summary>
//    /// 关闭对话面板/对话结束；
//    /// </summary>
//    void Close();

//}