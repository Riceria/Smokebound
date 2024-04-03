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

    // Adjacent triggers should be 2 x-values apart
    [SerializeField] private Vector3 teleportLocation;
    [SerializeField] private bool isHorizontalTransition;

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
        float x_offset = 0.5f;
        float y_offset = 0.3f;

        if (tilemapManager.currentTilemap == destination) {
            return;
        }
        
        Debug.Log("TransitionToTilemap(): ENTERING");
        tilemapManager.playerStatus.isTeleporting = true;
        // Enable renderer component of destination tilemap
        destination.GetComponent<TilemapRenderer>().enabled = true;

        // Update the current tilemap in TileMapManager
        tilemapManager.SetCurrentTilemap(destination);

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

        // teleportLocation should be an int
        Vector3 tempTeleport = new(teleportLocation.x, teleportLocation.y, player.transform.position.z);
        Debug.Log("tempTeleport (original): " + tempTeleport);
        if (isHorizontalTransition) {
            // Keep same vertical position
            tempTeleport.y = player.transform.position.y;
            tempTeleport.x += x_offset;
        } else {
            tempTeleport.x = player.transform.position.x;
            tempTeleport.y += y_offset;
        }
        Debug.Log("tempTeleport (modified): " + tempTeleport);
        player.transform.position = tempTeleport;

        // tilemapManager.playerStatus.isTeleporting = false;
        Debug.Log("TransitionToTilemap(): EXITING");
    }
}
