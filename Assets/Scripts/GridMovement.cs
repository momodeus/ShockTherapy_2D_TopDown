using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores the collision grid and performs functions related to it. 
/// </summary>
public class GridMovement : MonoBehaviour
{
    public TextAsset collisionMap; //text representation of map, with 1 being a wall and 0 being a path. 
                                   //importantly, make sure there is no extra line at the end of file. 
    public float gridSize = 0.48f; //should be ~0.48
    public float offsetX, offsetY; //x-y offset to visually position objects on map
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
    [HideInInspector]
    private int gridWidth, gridHeight; //width and height of grid
    private bool[,] collisions;     //boolean representation of text file
    private List<Vector2> validPoints; //all locations on collisions that are 0
    
    //We want to do all this loading before anything else in the scene
    void Awake()
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

        //finding valid points
        validPoints = new List<Vector2>();
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (collisions[j, i]) validPoints.Add(new Vector2(j, i));
            }
        }
        Debug.Log("finished awake with " + gridWidth + "x" + gridHeight + " grid, and " + validPoints.Count + " valid locations");


    }

    // Update is called once per frame
    void Update()
    {
        //might want to keep track of all cars here somehow, or to update some game manager stuff
    }

    /// <summary>
    /// Checks if a grid location is a wall or a path
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>true if path, false if wall</returns>
    public bool IsUnBlocked(int gridX, int gridY)
    {
        return IsValid(gridX, gridY) && collisions[gridX, gridY];
    }

    /// <summary>
    /// Checks if a grid location is inside the map
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>true if in bounds of map, false if out of bounds</returns>
    public bool IsValid(int gridX, int gridY)
    {
        return gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight;
    }

    /// <summary>
    /// Tests if an entity at gridX, gridY can move in the direction indicated by heading.
    /// </summary>
    /// <param name="heading">direction to move</param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>true if it can move, false if it is blocked. </returns>
    public bool CanMove(int heading, int gridX, int gridY)
    {
        if (gridX >= 1 && gridX < gridWidth-1 && gridY >= 1 && gridY < gridHeight-1)
        {
            switch(heading)
            {
                case NORTH:
                    return collisions[gridX, gridY + 1];
                case WEST:
                    return collisions[gridX - 1, gridY];
                case SOUTH:
                    return collisions[gridX, gridY - 1];
                case EAST:
                    return collisions[gridX + 1, gridY];
                default:
                    return false;
            }
        }
        return false;
    }

    
    public List<Vector2> GetValidPoints()
    {
        return validPoints;
    }

    /// <summary>
    /// Method to get worldspace coords for gridspace coords
    /// </summary>
    /// <param name="gx"></param>
    /// <param name="gy"></param>
    /// <returns>worldspace coords for given gridspace coords</returns>
    public Vector3 GetCoords(int gx, int gy)
    {
        return new Vector3(gridSize * (gx - 0.5f*GetWidth()) + offsetX, gridSize * (gy - 0.5f*GetHeight()) + offsetY, 0);
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
