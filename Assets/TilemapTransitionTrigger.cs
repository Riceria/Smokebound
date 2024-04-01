using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTransitionTrigger : MonoBehaviour
{
    public Tilemap destinationTilemap;
    public TilemapManager tilemapManager;
    private GameObject player;

    void Awake() {
        player = GameObject.Find("Player");
    } 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tilemapManager.playerStatus.isTeleporting = true;
            // Transition to destination tilemap
            if (tilemapManager.GetCurrentTilemap() != destinationTilemap) {
                TransitionToTilemap(destinationTilemap);
                
                // Teleport player to destination tilemap
                Vector3 teleportPosition = tilemapManager.currentTilemap.cellBounds.center;
                other.transform.position = new Vector3(20f, teleportPosition.y, other.transform.position.z);
            }
            tilemapManager.playerStatus.isTeleporting = false;
        }
    }

    private void TransitionToTilemap(Tilemap destination)
    {
        // Disable renderer component of current tilemap
        Tilemap currentTilemap = tilemapManager.GetCurrentTilemap();
        // currentTilemap.GetComponent<TilemapRenderer>().enabled = false;

        // Enable renderer component of destination tilemap
        destination.GetComponent<TilemapRenderer>().enabled = true;

        // Update the current tilemap in TileMapManager
        tilemapManager.UpdateCurrentTilemap(destination);

        // Handle camera transition
        CinemachineVirtualCamera[] cams = tilemapManager.cams;
        foreach (CinemachineVirtualCamera cam in cams)
        {
            if (cam.GetComponent<TilemapCameraReference>().associatedTilemap == destination)
            {
                // Enable the camera associated with the destination tilemap
                cam.enabled = true;
                cam.Follow = player.transform;
            }
            else
            {
                // Disable all other cameras
                cam.enabled = false;
            }
        }
    }
}
