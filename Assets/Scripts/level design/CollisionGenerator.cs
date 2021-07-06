using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionGenerator : MonoBehaviour
{
    public const int gridWidth = 35, gridHeight = 58;
    private const int borderThickness = 5;


    public static void CreateFromScratch(MapData mapData)
    {
        mapData.baseTilemap.ClearAllTiles();
        mapData.carsTilemap.ClearAllTiles();
        for (int i = -borderThickness; i < gridHeight + borderThickness; i++)
        {
            for (int j = -borderThickness; j < gridWidth + borderThickness; j++)
            {
                if (i >= 0 && i < gridHeight && j >= 0 && j < gridWidth)
                {
                    mapData.baseTilemap.SetTile(new Vector3Int(j - gridWidth / 2, -i + gridHeight / 2, 0), mapData.pathTile);
                } else
                {
                    mapData.baseTilemap.SetTile(new Vector3Int(j - gridWidth / 2, -i + gridHeight / 2, 0), mapData.borderTile);
                }
            }
        }
        mapData.carsTilemap.SetTile(new Vector3Int(0, 0, 0), mapData.playerTile);

        mapData.baseTilemap.CompressBounds();
    }
    public static void ReadCollisionMap(string collisionMap, MapData mapData)
    {
        mapData.baseTilemap.ClearAllTiles();
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.Split("\n"[0]));
        for (int i = -borderThickness; i < eachLine.Count + borderThickness; i++)
        {
            for (int j = -borderThickness; j < eachLine[0].Length + borderThickness; j++)
            {
                mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[0].Length / 2, -i + eachLine.Count / 2, 0), mapData.borderTile);
                if (i >= 0 && i < eachLine.Count && j >= 0 && j < eachLine[(int)Mathf.Max(0, Mathf.Min(eachLine.Count-1, i))].Length)
                {
                    if (eachLine[i][j] == '1')
                        mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.dirtTile);
                    if (eachLine[i][j] == '0' || eachLine[i][j] == 'E' || eachLine[i][j] == 'P')
                        mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.pathTile);
                    if (eachLine[i][j] == 'E')
                    {
                        mapData.enemyPositions.Enqueue(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0));
                        if(mapData.creatorMode) mapData.carsTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.enemyTile);
                    }
                    if (eachLine[i][j] == 'P')
                    {
                        mapData.SetPlayerPosition(j - eachLine[i].Length / 2, -i + eachLine.Count / 2);
                        if (mapData.creatorMode) mapData.carsTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.playerTile);
                    }
                }
            }
        }
        mapData.baseTilemap.CompressBounds();
    }

    public static string CreateCollisionMap(MapData mapData)
    {
        mapData.baseTilemap.CompressBounds();
        Debug.Log("width: " + (mapData.baseTilemap.cellBounds.xMax - mapData.baseTilemap.cellBounds.xMin) +
            "\nheight: " + (mapData.baseTilemap.cellBounds.yMax - mapData.baseTilemap.cellBounds.yMin));

        string col = "";
        for (int j = mapData.baseTilemap.cellBounds.yMax - 1 - borderThickness; j >= mapData.baseTilemap.cellBounds.yMin + borderThickness; j--)
        {
            for (int i = mapData.baseTilemap.cellBounds.xMin + borderThickness; i < mapData.baseTilemap.cellBounds.xMax - borderThickness; i++)
            {
                if (!mapData.baseTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Border Tile"))
                {
                    if (mapData.carsTilemap.HasTile(new Vector3Int(i, j, 0)))
                        col += mapData.carsTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Enemy Tile") ? "E" : "P";
                    else
                        col += mapData.baseTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Path Tile") ? "0" : "1";
                }
            }
            if (j > mapData.baseTilemap.cellBounds.yMin + borderThickness) col += "\n";
        }
        return col;
    }
    public static void CreateAndSaveCollisionMap(MapData mapData)
    {
        GameManager.Instance.SetCollisionMap(CreateCollisionMap(mapData));
    }
}