using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public string charaterID;

    [TextArea(6,6)] public string description;

    public List<SkillSO> skills;
}
