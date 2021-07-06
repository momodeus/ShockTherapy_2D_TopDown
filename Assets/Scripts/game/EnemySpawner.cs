using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GridMovement grid;
    public PlayerMovement player;
    public GameObject enemyPrefab;

    private List<EnemyMovement> enemies = new List<EnemyMovement>();
    // Start is called before the first frame update
    void Start()
    {
        List<Vector2> spawnPositions = grid.GetEnemySpawns();
        foreach (Vector2 pos in spawnPositions)
        {
            EnemyMovement em = (Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent(typeof(EnemyMovement)) as EnemyMovement);
            em.grid = grid;
            em.player = player;
            em.SetGridPosition((int)pos.x, (int)pos.y);
            em.source = this;
            enemies.Add(em);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasConflict(int heading, int gridX, int gridY)
    {
        int nx = gridX + (heading % 2 == 0 ? 0 : (heading == GridMovement.EAST ? 1 : -1));
        int ny = gridY + (heading % 2 == 0 ? (heading == GridMovement.NORTH ? 1 : -1) : 0);
        foreach (EnemyMovement em in enemies)
        {
            if (em.gridX == nx && em.gridY == ny) return true;
        }
        return false;
    }
}
