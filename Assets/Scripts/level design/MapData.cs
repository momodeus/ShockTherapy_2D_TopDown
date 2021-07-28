using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapData : MonoBehaviour
{
    public const int NUM_THEMES = 5;
    public Tilemap baseTilemap;
    public Tilemap carsTilemap;
    public RuleTile[] dirtTiles = new RuleTile[NUM_THEMES], pathTiles = new RuleTile[NUM_THEMES], borderTiles = new RuleTile[NUM_THEMES];
    public Tile playerTile, enemyTile;
    [HideInInspector]
    public Vector3Int playerPosition;
    [HideInInspector]
    public Queue<Vector3Int> enemyPositions = new Queue<Vector3Int>(9);
    [HideInInspector]
    public bool hasPlayer = false;

    public RuleTile GetBorderTile()
    {
        return borderTiles[GameManager.Instance.GetSelectedTheme()];
    }
    public RuleTile GetDirtTile()
    {
        return dirtTiles[GameManager.Instance.GetSelectedTheme()];
    }
    public RuleTile GetPathTile()
    {
        return pathTiles[GameManager.Instance.GetSelectedTheme()];
    }
    public void CullEnemies(int enemiesCount)
    {
        while (enemyPositions.Count > enemiesCount)
        {
            carsTilemap.SetTile(enemyPositions.Dequeue(), null);
        }
    }

    public void SetPlayerPosition(int x, int y)
    {
        playerPosition = new Vector3Int(x, y, 0);
        hasPlayer = true;
    }
}
