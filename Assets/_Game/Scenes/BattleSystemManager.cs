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
    }

    IEnumerator BeginBattle()
    {
        yield return new WaitForSeconds(1);
    }
}
