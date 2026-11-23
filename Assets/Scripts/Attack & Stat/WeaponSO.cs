using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponSo")]
public class WeaponSO : ScriptableObject
{
    public string ID;

    public GameObject prefab;

    public float attack;

    public int fireRate;

    public int maxBullet;

    public float maxDirection;

    public ArmStats armStat;

}