using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimation : MonoBehaviour
{
    public Animator animator;

    void Awake() {
        animator = gameObject.GetComponent<Animator>();
    }

    public void Attack() {
        animator.SetBool("isAttacking", true);
    }

    public void Magic() {
        animator.SetBool("isCasting", true);
    }

    public void Reset() {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isCasting", false);
    }

    public void Hurt() {
        animator.SetBool("attacked?", true);
    }

    public void SetHealth(float health) {
        animator.SetFloat("Health", health);
    }

    public void Idle() {
        animator.SetBool("attacked?", false);
    }

}
