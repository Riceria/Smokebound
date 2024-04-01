using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    // public GameObject effectPrefab;
    public float delayBeforeDestroy = 1f;

    // Called by animation event
    public void InstantiateEffect()
    {
        // Instantiate the effect at the position of this GameObject
        // GameObject effectInstance = Instantiate(effectPrefab, transform.position, Quaternion.identity);

        // Destroy the effect after a specified delay
        Destroy(gameObject, delayBeforeDestroy);
    }
}
