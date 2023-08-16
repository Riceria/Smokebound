using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    void Start()
    {
        enemyData.charName = "Triangle";
        enemyData.characterGameObject = transform.GetChild(0).gameObject;
        enemyData.level = 1;
        enemyData.maxHealth = 100;
        enemyData.maxMana = 100;
        enemyData.health = 100;
        enemyData.mana = 100;
    }
}
