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
public enum MissionStates
{
    Locked,
    Available,
    Active,
    Complete,
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