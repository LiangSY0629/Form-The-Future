using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string ID;

    public void SufferDamage(float damage)
    {
        Debug.Log(damage);
    }
}
