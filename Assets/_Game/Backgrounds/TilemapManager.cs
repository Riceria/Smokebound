using Cinemachine;
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

    public Tilemap GetCurrentTilemap() {
        return currentTilemap;
    }

    public void UpdateCurrentTilemap(Tilemap newTilemap) {
        currentTilemap = newTilemap;
    }

    private bool IsWithinTilemapBounds(Tilemap tilemap, Vector3 playerPosition)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3 tilemapCenter = tilemap.transform.position + new Vector3(bounds.center.x, bounds.center.y, 0f);

        // Check if player's position is within the bounds of the tilemap
        return playerPosition.x >= tilemapCenter.x + bounds.min.x &&
               playerPosition.x <= tilemapCenter.x + bounds.max.x &&
               playerPosition.y >= tilemapCenter.y + bounds.min.y &&
               playerPosition.y <= tilemapCenter.y + bounds.max.y;
    }
}
