using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAI : MonoBehaviour
{
    public Transform target; // target position
    Vector2 playerPos, enemyPos;

    void Update()
    {
        // Chase or run away from the Player within a set distance
        playerPos = new Vector2(target.localPosition.x, target.localPosition.y);
        enemyPos = new Vector2(this.transform.position.x, this.transform.localPosition.y);

        if (Vector3.Distance(transform.transform.position, target.transform.position) > 1.3)
        {
            transform.position = Vector2.MoveTowards(enemyPos, playerPos, 2 * Time.deltaTime);
        }

        if (Vector3.Distance(transform.transform.position, target.transform.position) < 1.15)
        {
            transform.position = Vector2.MoveTowards(enemyPos, playerPos, -1 * Time.deltaTime);
        }
    }
}
