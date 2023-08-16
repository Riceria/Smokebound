using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public PlayerData playerData;
    private EnemyData enemyData;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (playerData.health > 0) {
            if (other.CompareTag("Enemy")) {
                Debug.Log(playerData.charName + " collided with " + other.GetComponent<Enemy>().enemyData.charName);
                if (!playerData.isAttacked) {
                    playerData.isAttacked = true;
                    Debug.Log(playerData.charName + " has been attacked!");
                    SetBattleData(other);
                    LevelLoader.instance.LoadLevel("BattleArena");
                }
            }
        }
    }

    private void SetBattleData(Collider2D other)
    {
        // Player Data
        playerData.currentPosition = GetComponent<Rigidbody2D>().position;

        // Enemy Data
        EnemyData status = other.gameObject.GetComponent<Enemy>().enemyData;
        Debug.Log(status.charName);
        Debug.Log(status.characterGameObject);
        Debug.Log(status.level);
        Debug.Log(status.health);
        Debug.Log(status.maxHealth);
        Debug.Log(status.mana);
        Debug.Log(status.maxMana);
        // enemyData.charName = status.charName;
        // enemyData.characterGameObject = status.characterGameObject.transform.GetChild(0).gameObject;
        // enemyData.level = status.level;
        // enemyData.health = status.health;
        // enemyData.maxHealth = status.maxHealth;
        // enemyData.mana = status.mana;
        // enemyData.maxMana = status.maxMana;
    }
}
