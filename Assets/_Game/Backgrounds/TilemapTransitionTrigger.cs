using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTransitionTrigger : MonoBehaviour
{
    public Tilemap destinationTilemap;
    public TilemapManager tilemapManager;
    public LayerMask playerLayer;
    private GameObject player;
    private CinemachineVirtualCamera[] cams;

    void Awake() {
        player = GameObject.Find("Player");
    }

    void Start() {
        gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    public Tilemap GetDestinationTilemap() {
        return destinationTilemap;
    }

    public void TransitionToTilemap(Tilemap destination)
    {
        Debug.Log("TransitionToTilemap(): ENTERING");
        tilemapManager.playerStatus.isTeleporting = true;
        // Enable renderer component of destination tilemap
        destination.GetComponent<TilemapRenderer>().enabled = true;

        // Update the current tilemap in TileMapManager
        tilemapManager.UpdateCurrentTilemap(destination);

        // Handle camera transition
        cams = tilemapManager.cams;
        foreach (CinemachineVirtualCamera cam in cams)
        {
            if (cam.GetComponent<TilemapCameraReference>().associatedTilemap == destination)
            {
                // Enable the camera associated with the destination tilemap
                Debug.Log("Enabling cam: " + cam);
                cam.enabled = true;
                cam.Follow = player.transform;
            }
            else
            {
                // Disable all other cameras
                Debug.Log("Disabling cam: " + cam);
                cam.enabled = false;
            }
        }
        // tilemapManager.playerStatus.isTeleporting = false;
        Debug.Log("TransitionToTilemap(): EXITING");
    }
}
