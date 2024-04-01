using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemManager : MonoBehaviour
{
    // State and statuses
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE, END }
    private BattleState battleState;
    public CharacterStatus playerStatus;
    public CharacterStatus enemyStatus;
    public Canvas battleUI;

    // Battle HUD
    public TextMeshProUGUI battleTurn;
    public TextMeshProUGUI playerHP;
    public TextMeshProUGUI enemyHP;

    // Player
    public GameObject playerPrefab;
    public BattleAnimation animator; 
    private bool isAttacking = false, isCasting = false, isPerformingAction, isAttacked;
    private bool isRunning = false; 
    public GameObject lightning;

    // Enemy
    public GameObject enemyPrefab;


    void Awake () {
        GameObject enemyInstance = Instantiate(enemyPrefab);
        enemyInstance.transform.parent = GameObject.Find("Enemies").transform;
        battleUI.GetComponent<Canvas>().enabled = false;
        
    }

    void Start()
    {
        playerStatus.health = playerStatus.maxHealth;
        enemyStatus.health = enemyStatus.maxHealth;
        battleState = BattleState.START;
        StartCoroutine(BeginBattle());
    }

    void Update() {
        // Update HUD
        playerHP.text = playerStatus.health.ToString();
        enemyHP.text = enemyStatus.health.ToString();
        battleTurn.text = battleState.ToString();
    }

    IEnumerator BeginBattle()
    {
        yield return new WaitForSeconds(1);
        battleUI.GetComponent<Canvas>().enabled = true;
        yield return StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        animator.Idle();
        battleState = BattleState.PLAYERTURN;
        Debug.Log("Player turn");
        yield return new WaitForSeconds(1);

        // Win or lose
        if (playerStatus.health <= 0) {
            battleState = BattleState.LOSE;
            yield return StartCoroutine(Lose());
        } else if (enemyStatus.health <= 0) {
            battleState = BattleState.WIN;
            yield return StartCoroutine(Win());
        }

        // Wait for player's action
        while (!isAttacking && !isCasting && !isRunning) {
            yield return null;
        }

        if (isAttacking || isCasting) {
            yield return StartCoroutine(PlayerAttack());
        } else if (isRunning) {
            yield return StartCoroutine(Run());
        }

        // Reset action flags
        isAttacking = false;
        isCasting = false;
        isAttacked = false;

        yield return StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy turn");
        yield return new WaitForSeconds(1);

        // Win or lose
        if (playerStatus.health <= 0) {
            battleState = BattleState.LOSE;
            yield return StartCoroutine(Lose());
        } else if (enemyStatus.health <= 0) {
            battleState = BattleState.WIN;
            yield return StartCoroutine(Win());
        } else if (playerStatus.health > 0) {
            int damage = UnityEngine.Random.Range(0, 50);
            Debug.Log(enemyStatus.charName + " attacked");
            animator.Hurt();
            float newHP = playerStatus.health - damage;
            animator.SetHealth(newHP);
            playerStatus.health -= damage;
            Debug.Log("Dealt " + damage + " damage");

            // Give a short delay before changing the battle state and yielding to PlayerTurn
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(PlayerTurn());
        } 
    }

    IEnumerator PlayerAttack() {
        isPerformingAction = true;
        int damage = 0;
        
        if (isAttacking) {
            Debug.Log("isAttacking: " + isAttacking);
            animator.Attack();
            damage = UnityEngine.Random.Range(5, 15);
            Debug.Log(playerStatus.charName + " attacked");
            yield return new WaitForSeconds(0.5f);
        }
        else if (isCasting) {
            Debug.Log("isCasting: " + isCasting);
            yield return new WaitForSeconds(0.1f);
            animator.Magic(); // Start the casting animation
            yield return new WaitForSeconds(2);

            // Instantiate the magic effect
            GameObject magicEffectInstance = Instantiate(lightning);
            magicEffectInstance.transform.parent =  GameObject.Find("MagicEffects").transform;
            
            // Deal damage or perform other actions related to casting
            damage = UnityEngine.Random.Range(0, 30);
            Debug.Log(playerStatus.charName + " used Thunder!");
            yield return new WaitForSeconds(0.5f);

            // Destroy the magic effect after the casting animation completes
            Destroy(magicEffectInstance);
        }   
        
        enemyStatus.health -= damage;
        Debug.Log("Dealt " + damage + " damage");

        // Reset action flags
        isAttacking = false;
        isCasting = false;
        isPerformingAction = false;   
        animator.Reset();

        // Give a short delay before changing the battle state and yielding to EnemyTurn
        yield return new WaitForSeconds(0.25f);     

        battleState = BattleState.ENEMYTURN;
        yield return StartCoroutine(EnemyTurn());
    }

    // Called by the ATTACK button
    public void AttackWrapper() {
        if (battleState == BattleState.PLAYERTURN && !isPerformingAction) {
            isAttacking = true;
        }
    }

    // Called by the MAGIC button
    public void MagicWrapper() {
        if (battleState == BattleState.PLAYERTURN && !isPerformingAction) {
            isCasting = true;
        }
    }

    // Called by the RUN button
    public void RunWrapper() {
        if (battleState == BattleState.PLAYERTURN && !isPerformingAction) {
            isRunning = true;
        }
    }

    IEnumerator Win()
    {
        Debug.Log("Battle won!");
        yield return new WaitForSeconds(1);
        Debug.Log("Returning to Overworld");
        LevelLoader.instance.LoadLevel("Overworld");
    }

    IEnumerator Lose()
    {
        Debug.Log(playerStatus.charName + " died");
        Debug.Log("Battle lost...");
        yield return new WaitForSeconds(1);
        battleUI.GetComponent<Canvas>().enabled = false;
        Debug.Log("Game over...");
        LevelLoader.instance.LoadLevel("GameOver");
    }

    IEnumerator Run()
    {
        isPerformingAction = true;
        playerStatus.isAttacked = false;
        Debug.Log(playerStatus.charName + " ran away");
        yield return new WaitForSeconds(1);
        battleUI.GetComponent<Canvas>().enabled = false;
        Debug.Log("Returning to Overworld");
        LevelLoader.instance.LoadLevel("Overworld");
    }
}
