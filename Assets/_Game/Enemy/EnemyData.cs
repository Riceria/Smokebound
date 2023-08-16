using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "StatusObjects/Enemy", order = 1)]
public class EnemyData : ScriptableObject
{
    public string charName = "enemy";
    public GameObject characterGameObject;
    public int level = 1;
    public float maxHealth = 100;
    public float maxMana = 100;
    public float health = 100;
    public float mana = 100;

}
