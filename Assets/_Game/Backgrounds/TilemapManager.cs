using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public CharacterStatus playerStatus;
    public Tilemap[] areaList;
    public CinemachineVirtualCamera[] cams; 
    public Tilemap currentTilemap;

    public void UpdatePlayerPosition(Vector3 playerPosition)
    {
        CheckTilemap(playerPosition);
        Debug.Log("Current tilemap: " + currentTilemap);
    }

    private void CheckTilemap(Vector3 playerPosition)
    {
        foreach (Tilemap area in areaList)
        {
            if (IsWithinTilemapBounds(area, playerPosition))
            {
                currentTilemap = area;
                break; // Exit loop once the current tilemap is found
            }
        }
    }

    public CinemachineVirtualCamera GetActiveCam() {
        foreach (CinemachineVirtualCamera cam in cams)
        {
            if (cam.isActiveAndEnabled) {
                // Debug.Log("Active cam: " + cam);
                // Debug.Log(currentTilemap);
                return cam;
            }
        }
        return null;
    }

    public void UpdateCurrentTilemap() {
        currentTilemap = GetActiveCam().GetComponent<TilemapCameraReference>().associatedTilemap;
    }

    public Tilemap GetCurrentTilemap() {
        return currentTilemap;
    }

    public void SetCurrentTilemap(Tilemap newTilemap) {
        currentTilemap = newTilemap;
    }

    private bool IsWithinTilemapBounds(Tilemap tilemap, Vector3 playerPosition)
    {
        BoundsInt bounds = tilemap.cellBounds;
        return !bounds.Contains(currentTilemap.WorldToCell(playerPosition)) && !playerStatus.isTeleporting;
    }
}
