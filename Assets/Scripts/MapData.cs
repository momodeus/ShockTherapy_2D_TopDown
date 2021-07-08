using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapData : MonoBehaviour
{
    public Tilemap baseTilemap;
    public Tilemap carsTilemap;
    public RuleTile dirtTile, pathTile, borderTile;
    public Tile playerTile, enemyTile;
    public Vector3Int playerPosition;
    public Queue<Vector3Int> enemyPositions;
    public bool hasPlayer = false;
    public bool creatorMode = true;
    void Awake()
    {
        enemyPositions = new Queue<Vector3Int>(9);
    }



    public void CullEnemies(int enemiesCount)
    {
        if (!creatorMode) return;
        while (enemyPositions.Count > enemiesCount)
        {
            carsTilemap.SetTile(enemyPositions.Dequeue(), null);
        }
    }

    public void SetPlayerPosition(int x, int y)
    {
        if (!creatorMode) return;
        playerPosition = new Vector3Int(x, y, 0);
        hasPlayer = true;
    }
}
