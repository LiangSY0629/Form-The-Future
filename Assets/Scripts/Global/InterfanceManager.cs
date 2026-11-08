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

    /// <summary>
    /// 获取到npc的闲聊；
    /// </summary>
    /// <returns></returns>
    MissionSO DailyDialogue();

}
