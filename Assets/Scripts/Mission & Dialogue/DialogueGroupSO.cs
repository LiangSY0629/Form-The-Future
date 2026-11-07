using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New DialogueGroup", menuName = "Mission/New Dialogue Group")]
public class DialogueGroupSO : ScriptableObject
{
    public string groupID;
    public string triggerNpcID;
    public List<DialogueParagraphSO> paragrapgSos;
    public List<UnlockCondition> conditions;

    public bool isSubtitle;      //是不是字幕形式显示；
    public bool isContinuous;    //要不要接着上一段播放（指面板切换至字幕，字幕切换至面板是否连续，还是有其他条件，暂时先用bool，如有需求可改enum
}
