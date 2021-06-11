using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSpawner : MonoBehaviour
{
    public GameObject flag;
    public const int numFlagsToSpawn = 10;
    public GridMovement grid;
    // Start is called before the first frame update
    void Start()
    {
        List<Vector2> possible = grid.GetValidPoints();
        int i = numFlagsToSpawn;
        while(i > 0 && possible.Count > 0)
        {
            int idx = (int)Random.Range(0, possible.Count - 0.001f);
            Instantiate(flag, grid.GetCoords((int)possible[idx].x, (int)possible[idx].y), Quaternion.identity);
            possible.RemoveAt(idx);
            i--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
