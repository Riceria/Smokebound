using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    public CharacterStatus playerStatus;
    public CharacterStatus enemyStatus;
    public CharacterStatus currentEnemy;

    void Start() {
        enemyStatus.characterGameObject = gameObject;
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log(playerStatus.charName + " collided with " + enemyStatus.charName);
            if (!playerStatus.isAttacked) {
                playerStatus.isAttacked = true;
                Debug.Log(playerStatus.charName + " has been attacked!");
                SetBattleData(collision);
                LevelLoader.instance.LoadLevel("BattleArena");
            }
        }
    }

    private void SetBattleData(Collider2D collision)
    {
        currentEnemy.charName = enemyStatus.charName;
        currentEnemy.level = enemyStatus.level;
        currentEnemy.maxHealth = enemyStatus.maxHealth;
        currentEnemy.maxMana = enemyStatus.maxMana;
        currentEnemy.health = enemyStatus.health;
        currentEnemy.mana = enemyStatus.mana;
    }
}
