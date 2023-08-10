using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public PlayerData playerData;
    public bool isAttacked = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (playerData.health > 0) {
            if (other.CompareTag("Enemy")) {
                Debug.Log(playerData.charName + " collided with " + other.GetComponent<Enemy>().enemyData.charName);
                if (!isAttacked) {
                    isAttacked = true;
                    //SetBattleData(other);
                    LevelLoader.instance.LoadLevel("BattleArena");
                }
            }
        }
    }

    // private void SetBattleData(Collider2D other)
    // {
    //     // Player Data
    //     playerStatus.position[0] = this.transform.position.x;
    //     playerStatus.position[1] = this.transform.position.y;

    //     // Enemy Data
    //     PlayerData status = other.gameObject.GetComponent<StatusManager>().enemyStatus;
    //     enemyStatus.charName = status.charName;
    //     enemyStatus.characterGameObject = status.characterGameObject.transform.GetChild(0).gameObject;  
    //     enemyStatus.health = status.health;
    //     enemyStatus.maxHealth = status.maxHealth;
    //     enemyStatus.mana = status.mana;
    //     enemyStatus.maxMana = status.maxMana;
    // }
}
