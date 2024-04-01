using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "StatusObjects/Battle Stats", order = 1)]
public class CharacterStatus : ScriptableObject
{
    public string charName;
    public Vector2 currentPosition;
    public Transform battlePosition;
    public bool isAttacked, isMoving, isTeleporting;
    public GameObject characterGameObject;
    public int level = 1;
    public float maxHealth = 100;
    public float maxMana = 100;
    public float health = 100;
    public float mana = 100;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    
}
