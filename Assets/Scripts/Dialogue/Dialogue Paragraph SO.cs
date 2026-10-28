using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Paragraph", menuName = "Dialogue/Dialogue Paragraph")]
public class DialogueParagraphSO : ScriptableObject
{
    public string paragraphID;
    public List<DialogueLineSO> dialogueParagraph;
    public string nextID; 

}
