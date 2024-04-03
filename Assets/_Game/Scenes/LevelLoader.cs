// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class LevelLoader : MonoBehaviour
// {
//     #region Singleton
//     public static LevelLoader instance;

//     void Awake()
//     {
//         instance = this;
//         DontDestroyOnLoad(this.gameObject);
//     }
//     #endregion

//     public Animator transition;
//     public float transitionTime = 1f;
 
//     public void LoadLevel(string levelName)
//     {
//         StartCoroutine(LoadNamedLevel(levelName));
//     }
    
//     IEnumerator LoadNamedLevel(string levelName)
//     {
//         // Start transition animation
//         transition.SetTrigger("Start");
    
//         yield return new WaitForSeconds(transitionTime);
    
//         SceneManager.LoadScene(levelName);
//         Debug.Log("Battle start! Loaded " + levelName);
    
//         // End transition animation
//         transition.SetTrigger("End");
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    #region Singleton
    public static LevelLoader instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public Animator transition;
    public float transitionTime = 1f;
    public GameObject player;

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadNamedLevel(levelName));
    }

    IEnumerator LoadNamedLevel(string levelName)
    {
        // player = GameObject.Find("Player");
        // Start transition animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        float xpos = PlayerPrefs.GetFloat("X");
        float ypos = PlayerPrefs.GetFloat("Y");
        float zpos = PlayerPrefs.GetFloat("Z");
        Vector3 playerPos = new(xpos, ypos, zpos);
        Debug.Log("playerPosition: " + playerPos);

        // Unload previous scene
        SceneManager.LoadScene(levelName);
        Debug.Log("Loaded " + levelName);
        Instantiate(player, playerPos, Quaternion.identity);

        // End transition animation
        transition.SetTrigger("End");
    }
}
