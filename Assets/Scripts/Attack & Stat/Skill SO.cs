using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill")]
public class SkillSO : ScriptableObject
{
    public string skillID;

    [TextArea] public string description;

    public float lockValue;

    public SkillEffectTarget effectTarget;

    public List<float> hurtValue;

    public List<Buff> buffs;

} 
