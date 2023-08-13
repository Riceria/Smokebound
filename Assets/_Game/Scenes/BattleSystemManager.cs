using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystemManager : MonoBehaviour
{
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    private BattleState battleState;
    
    void Start()
    {
        battleState = BattleState.START;
        StartCoroutine(BeginBattle());
        // StartCoroutine(PlayerTurn());
        // StartCoroutine(EnemyTurn());
        // StartCoroutine(Win());
        // StartCoroutine(Lose());
    }

    IEnumerator BeginBattle()
    {
        yield return new WaitForSeconds(1);
    }

    // IEnumerator PlayerTurn()
    // {
    //     yield return new WaitForSeconds(1);
    // }

    // IEnumerator EnemyTurn()
    // {
    //     yield return new WaitForSeconds(1);
    // }

    // IEnumerator Win()
    // {
    //     yield return new WaitForSeconds(1);
    // }

    // IEnumerator Lose()
    // {
    //     yield return new WaitForSeconds(1);
    // }
}
