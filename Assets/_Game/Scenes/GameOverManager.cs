using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public CharacterStatus playerStatus;

    public void Continue() {
        playerStatus.health = playerStatus.maxHealth;
        Debug.Log("Continuing");
        Debug.Log("Returning to Overworld");
        LevelLoader.instance.LoadLevel("Overworld");
        
    }

    public void Quit() {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
