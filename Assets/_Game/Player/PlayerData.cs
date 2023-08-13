using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "StatusObjects/Player", order = 1)]
public class PlayerData : ScriptableObject
{
    public string charName;
    public Vector2 currentPosition;
    public bool isAttacked;
    //public GameObject characterGameObject;
    public int level = 1;
    public float maxHealth = 100;
    public float maxMana = 100;
    public float health = 100;
    public float mana = 100;
}
