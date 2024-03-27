using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemManager : MonoBehaviour
{
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    private BattleState battleState;

    public TextMeshProUGUI battleTurn;
    public TextMeshProUGUI playerHP;
    public TextMeshProUGUI enemyHP;

    public GameObject playerPrefab;
    private Animator animator;
    private bool isAttacking = false, isCasting = false, isPerformingAction;

    public GameObject enemyPrefab;

    private Vector3 startPosition = new(0, 0 ,0);

    public CharacterStatus playerStatus;
    public CharacterStatus enemyStatus;

    void Awake () {
        Instantiate(playerPrefab, startPosition, Quaternion.identity);
        Instantiate(enemyPrefab);
        animator = playerPrefab.GetComponent<Animator>();
    }

    void Start()
    {
        playerStatus.health = playerStatus.maxHealth;
        enemyStatus.health = enemyStatus.maxHealth;
        battleState = BattleState.START;
        // playerBattle = playerPrefab.transform.Find("PlayerBattle").gameObject;
        // playerPrefab.SetActive(true);
        // playerPrefab.transform.Find("Player").gameObject.SetActive(false);
        // playerPrefab.transform.Find("PlayerBattle").gameObject.SetActive(true);
        // animator = playerPrefab.transform.Find("PlayerBattle").gameObject.GetComponent<Animator>();

        if (animator.gameObject.activeSelf) {
            Debug.Log("animator.gameObject.activeSelf: " + animator.gameObject.activeSelf);
        }

        if (animator != null) {
            Debug.Log("animator not null");
            Debug.Log(animator.gameObject.name);
        }
        StartCoroutine(BeginBattle());
    }

    void Update() {
        playerHP.text = playerStatus.health.ToString();
        enemyHP.text = enemyStatus.health.ToString();
        battleTurn.text = battleState.ToString();
        // playerHP.text = playerStatus.health.ToString() + " / " + playerStatus.maxHealth.ToString();
        // enemyHP.text = enemyStatus.health.ToString() + " / " + enemyStatus.maxHealth.ToString();
    }

    IEnumerator BeginBattle()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        battleState = BattleState.PLAYERTURN;
        Debug.Log("Player turn");
        yield return new WaitForSeconds(1);

        while (!isAttacking && !isCasting) {
            yield return null;
        }

        if (isAttacking || isCasting) {
            yield return StartCoroutine(PlayerAttack());
        }

        isAttacking = false;
        isCasting = false;

        if (playerStatus.health <= 0 || enemyStatus.health <= 0) {
            StopAllCoroutines();
        }

        if (playerStatus.health <= 0) {
            yield return StartCoroutine(Lose());
        } else if (enemyStatus.health <= 0) {
            yield return StartCoroutine(Win());
        } else {
            yield return StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy turn");
        yield return new WaitForSeconds(1);

        if (playerStatus.health <= 0 || enemyStatus.health <= 0) {
            StopAllCoroutines();
        }

        if (playerStatus.health <= 0) {
            yield return StartCoroutine(Lose());
        } else if (enemyStatus.health <= 0) {
            yield return StartCoroutine(Win());
        }

        if (playerStatus.health > 0) {
            int damage = UnityEngine.Random.Range(0, 20);
            Debug.Log(enemyStatus.charName + " attacked");
            playerStatus.health -= damage;
            Debug.Log("Dealt " + damage + " damage");

            // Give a short delay before changing the battle state and yielding to PlayerTurn
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(PlayerTurn());
        } 
    }

    IEnumerator Win()
    {
        battleState = BattleState.WIN;
        Debug.Log("Battle won!");
        yield return new WaitForSeconds(1);
        Debug.Log("Returning to Overworld");
        LevelLoader.instance.LoadLevel("Overworld");
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

    IEnumerator PlayerAttack() {
        isPerformingAction = true;
        int damage = 0;
        
        if (isAttacking) {
            Debug.Log("isAttacking: " + isAttacking);
            animator.SetBool("isAttacking", isAttacking);
            damage = UnityEngine.Random.Range(5, 15);
            Debug.Log(playerStatus.charName + " attacked");
        }
        else if (isCasting) {
            Debug.Log("isCasting: " + isCasting);
            animator.SetBool("isCasting", isCasting);
            damage = UnityEngine.Random.Range(0, 30);
            Debug.Log(playerStatus.charName + " used Thunder!");
        }   
        
        enemyStatus.health -= damage;
        Debug.Log("Dealt " + damage + " damage");

        // Give a short delay before changing the battle state and yielding to EnemyTurn
        yield return new WaitForSeconds(0.5f);

        // Reset action flags
        isAttacking = false;
        isCasting = false;
        isPerformingAction = false;        

        battleState = BattleState.ENEMYTURN;
        yield return StartCoroutine(EnemyTurn());
    }


    IEnumerator Lose()
    {
        Debug.Log(playerStatus.charName + " died");
        Debug.Log("Battle lost...");
        yield return new WaitForSeconds(1);
        Debug.Log("Returning to Overworld");
        LevelLoader.instance.LoadLevel("Overworld");
    }
}
