using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores the collision grid and performs functions related to it. 
/// </summary>
public class GridMovement
{

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
    private static int gridWidth, gridHeight; //width and height of grid
    private static bool[,] collisions;     //boolean representation of text file
    private static List<Vector2Int> validPoints; //all locations on collisions that are 0
    private static List<Vector2Int> enemySpawns;
    private static Vector2Int playerSpawn;

    public static void LoadMap(string map)
    {
        enemySpawns = new List<Vector2Int>();

        //reading collision file and setting up collision matrix
        List<string> eachLine = new List<string>();
        eachLine.AddRange(map.Split("\n"[0]));
        collisions = new bool[eachLine[0].Length, eachLine.Count];
        for (int i = 0; i < eachLine.Count; i++)
        {
            for (int j = 0; j < eachLine[i].Length; j++)
            {
                collisions[j, eachLine.Count - (i + 1)] = eachLine[i][j] == '0' || eachLine[i][j] == 'P' || eachLine[i][j] == 'E'; //inserting lines in inverse order, to keep +y facing North. 
                if(eachLine[i][j] == 'E')
                {
                    enemySpawns.Add(new Vector2Int(j, eachLine.Count - (i + 1)));
                }
                if(eachLine[i][j] == 'P')
                {
                    playerSpawn = new Vector2Int(j, eachLine.Count - (i + 1));
                }
            }
        }
        gridWidth = collisions.GetLength(0);
        gridHeight = collisions.GetLength(1);

        //finding valid points
        validPoints = new List<Vector2Int>();
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (collisions[j, i]) validPoints.Add(new Vector2Int(j, i));
            }
        }
    }

    /// <summary>
    /// Checks if a grid location is a wall or a path
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>true if path, false if wall</returns>
    public static bool IsUnBlocked(int gridX, int gridY)
    {
        return IsValid(gridX, gridY) && collisions[gridX, gridY];
    }

    /// <summary>
    /// Checks if a grid location is inside the map
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>true if in bounds of map, false if out of bounds</returns>
    public static bool IsValid(int gridX, int gridY)
    {
        return gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight;
    }


    public static Vector2Int GetPlayerSpawn()
    {
        return playerSpawn;
    }

    public static List<Vector2Int> GetEnemySpawns()
    {
        return enemySpawns;
    }

    /// <summary>
    /// Tests if an entity at gridX, gridY can move in the direction indicated by heading.
    /// </summary>
    /// <param name="heading">direction to move</param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>true if it can move, false if it is blocked. </returns>
    public static bool CanMove(int heading, int gridX, int gridY)
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

    
    public static List<Vector2Int> GetValidPoints()
    {
        return validPoints;
    }

    /// <summary>
    /// Method to get worldspace coords for gridspace coords
    /// </summary>
    /// <param name="gx"></param>
    /// <param name="gy"></param>
    /// <returns>worldspace coords for given gridspace coords</returns>
    public static Vector3 GetCoords(int gx, int gy)
    {
        return new Vector3((gx - 0.5f*GetWidth()), (gy - 0.5f*GetHeight()), 0);
    }
    public static int GetWidth()
    {
        return gridWidth;
    }

    public static int GetHeight()
    {
        return gridHeight;
    }
}
