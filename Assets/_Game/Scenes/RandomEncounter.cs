using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomEncounter : MonoBehaviour
{
    public CharacterStatus playerStatus;
    public CharacterStatus enemyStatus;
    public CharacterStatus currentEnemy; // change this later to choose from a list of enemies
    [SerializeField] private TilemapManager tilemapManager;
    private bool isChecking = false;
    private bool isSafeZone = false;
    private List<Tilemap> safeZones;

    void Start() {
        enemyStatus.characterGameObject = gameObject;
        // Get tilemap manager script from Grid
        tilemapManager = gameObject.GetComponent<TilemapManager>();

        // Should be Main/AquaSS/GrassSS/IcySS/RockSS
        safeZones = new List<Tilemap>();
        for (int i = 0; i <= tilemapManager.areaList.Length; i+=2) {
            safeZones.Add(tilemapManager.areaList[i]);
        }
    }

    void FixedUpdate() {
        // Only perform random encounter check if certain conditions are met

        if (safeZones.Contains(tilemapManager.GetCurrentTilemap())) {
            isSafeZone = true;
        } else {
            isSafeZone = false;
        }

        // Debug.Log("isSafeZone: " + isSafeZone);

        if (CanCheckForRandomEncounter()) {
            StartCoroutine(RandomEncounterCheck());
        }
    }

    bool CanCheckForRandomEncounter() {
        // Only check for random encounters if player is healthy, not already attacked, and is moving
        return playerStatus.health > 0 && !playerStatus.isAttacked &&
               playerStatus.isMoving && !isChecking && !isSafeZone;
    }

    IEnumerator RandomEncounterCheck() {
        isChecking = true;
        yield return new WaitForSeconds(0.5f); // Add a delay to prevent rapid checks

        float encounterRate = UnityEngine.Random.Range(0, 500);
        Debug.Log("Encounter Rate: " + encounterRate);

        if (encounterRate <= 10) {
            // 10% chance to trigger random encounter every 0.5 seconds
            playerStatus.isAttacked = true;
            Debug.Log(playerStatus.charName + " has been attacked!");
            SetBattleData();
            PlayerPrefs.SetFloat("X", playerStatus.currentPosition.x);
            PlayerPrefs.SetFloat("Y", playerStatus.currentPosition.y);
            LevelLoader.instance.LoadLevel("BattleArena");
        } 

        isChecking = false;
    }

    private void SetBattleData()
    {
        currentEnemy.charName = enemyStatus.charName;
        currentEnemy.level = enemyStatus.level;
        currentEnemy.maxHealth = enemyStatus.maxHealth;
        currentEnemy.maxMana = enemyStatus.maxMana;
        currentEnemy.health = enemyStatus.health;
        currentEnemy.mana = enemyStatus.mana;
    }
}
