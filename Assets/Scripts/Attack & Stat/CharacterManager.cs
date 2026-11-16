using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public List<CharacterSO> characterSos;

    public Dictionary<string, CharacterSO> characterSoDictionary;

    public CharacterRunTime currentCharacter;

    Dictionary<string, float> affinityDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        affinityDictionary = new();
        characterSoDictionary = new();

        foreach (CharacterSO character in characterSos)
        {
            characterSoDictionary[character.charaterID] = character;
            affinityDictionary[character.charaterID] = 0;
        }
    }

    /// <summary>
    /// 传入ID获取对应角色好感度
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public float GetAffinityValue(string ID)
    {
        affinityDictionary.TryGetValue(ID, out float value);

        return value;
    }


    /// <summary>
    /// 传入ID和值，让当前好感度提升对应值；
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public void SetAffinityValue(string ID, float value) => affinityDictionary[ID] += value;


}


public class CharacterRunTime
{
    public string ID;

    public CharacterSO characterSo;
}