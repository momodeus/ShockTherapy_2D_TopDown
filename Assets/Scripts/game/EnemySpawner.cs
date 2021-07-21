using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static List<EnemyMovement> enemies;
    
    public static void SpawnEnemies(GameObject enemyPrefab, PlayerMovement player)
    {
        enemies = new List<EnemyMovement>();
        List<Vector2Int> spawnPositions = GridMovement.GetEnemySpawns();
        foreach (Vector2Int pos in spawnPositions)
        {
            EnemyMovement em = (Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent(typeof(EnemyMovement)) as EnemyMovement);
            em.player = player;
            em.SetGridPosition(pos.x, pos.y);
            enemies.Add(em);
        }
    } 

    public static bool HasConflict(int heading, int gridX, int gridY)
    {
        int nx = gridX + (heading % 2 == 0 ? 0 : (heading == GridMovement.EAST ? 1 : -1));
        int ny = gridY + (heading % 2 == 0 ? (heading == GridMovement.NORTH ? 1 : -1) : 0);
        foreach (EnemyMovement em in enemies)
        {
            if (!em.allowedToMove && em.gridX == nx && em.gridY == ny) return true;
        }
        return false;
    }
}
