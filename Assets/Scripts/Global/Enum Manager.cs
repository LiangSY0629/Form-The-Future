/// <summary>
/// 任务的类型；
/// </summary>
public enum MissionTypes
{
    Main,
    Side,
    Daily,
}

/// <summary>
/// 任务的状态；
/// </summary>
[System.Serializable]
public enum MissionStates
{
    Locked = 0,
    Available = 1,
    Active = 2,
    Complete = 3,
}

/// <summary>
/// 对话时选项的跳转类型
/// </summary>
public enum ChooseTypes 
{
    Mission,
    Group,
    Paragraph,
}

/// <summary>
/// 结束本对话段后对本对话组的影响；
/// </summary>
public enum ParagraphOutCome
{
    Default,
    Abort,
}

public enum ModifierTypes
{
    Direct,
    Percent,
}