using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public TextAsset collisionMap;
    public float gridSize = 0.48f; //should be ~0.48
    public float offsetX, offsetY;
    //direction constants (don't want to see in inspector, but valuable to other places)
    [HideInInspector]
    public const int NORTH = 0;
    [HideInInspector]
    public const int WEST = 1;
    [HideInInspector]
    public const int SOUTH = 2;
    [HideInInspector]
    public const int EAST = 3;

    [HideInInspector]
    public const int NONE = -1;

    private int gridWidth, gridHeight;
    private bool[,] collisions;

    // Start is called before the first frame update
    void Start()
    {
        //reading collision file and setting up collision matrix
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.text.Split("\n"[0]));
        collisions = new bool[eachLine[0].Length, eachLine.Count];
        for (int i = 0; i < eachLine.Count; i++)
        {
            for (int j = 0; j < eachLine[i].Length; j++)
            {
                collisions[j, eachLine.Count - (i + 1)] = eachLine[i][j] == '0'; //inserting lines in inverse order, to keep +y facing North. 
            }
        }
        gridWidth = collisions.GetLength(0);
        gridHeight = collisions.GetLength(1);
    }

    // Update is called once per frame
    void Update()
    {
        //might want to keep track of all cars here somehow, or to update some game manager stuff
    }


    public bool CanMove(int heading, int gridX, int gridY)
    {
        if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight)
        {
            switch(heading)
            {
                case NORTH:
                    return collisions[gridX, gridY + 1];
                case EAST:
                    return collisions[gridX + 1, gridY];
                case SOUTH:
                    return collisions[gridX, gridY - 1];
                case WEST:
                    return collisions[gridX - 1, gridY];
                default:
                    return false;
            }
        }
        return false;
    }

    public int GetWidth()
    {
        return gridWidth;
    }

    public int GetHeight()
    {
        return gridHeight;
    }
    public float GetGridSize()
    {
        return gridSize;
    }
}
