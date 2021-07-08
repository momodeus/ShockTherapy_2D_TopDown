using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner for flags at the beginning of a game. 
/// </summary>
public class FlagSpawner : MonoBehaviour
{
    public GameObject flag; //reference to flag prefab
    public LocalGameManager gameManager;
    public GridMovement grid; //reference to map's grid
    // Start is called before the first frame update
    void Start()
    {
        SpawnFlags(gameManager.numFlagsToSpawn);
    }

    void SpawnFlags(int numFlagsToSpawn)
    {
        List<Vector2> possible = grid.GetValidPoints();
        GameManager.Instance.SetFlagsRemaining(numFlagsToSpawn);
        int i = numFlagsToSpawn;
        while (i > 0 && possible.Count > 0)
        {
            int idx = (int)Random.Range(0, possible.Count - 0.001f);
            GridObject go = (Instantiate(flag, Vector3.zero, Quaternion.identity).GetComponent(typeof(GridObject)) as GridObject);
            go.grid = grid;
            go.SetGridPosition((int)possible[idx].x, (int)possible[idx].y);
            possible.RemoveAt(idx);
            i--;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
