using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionGenerator : MonoBehaviour
{
    public const int gridWidth = 34, gridHeight = 58;
    private const int borderThickness = 5;

    
    public static void CreateFromScratch(MapData mapData)
    {
        ReadCollisionMap(BlankCollisionMap(), mapData);
    }

    public static string BlankCollisionMap()
    {
        string collisionMap = "";
        for (int i = 0; i < gridHeight; i++)
        {
            collisionMap += "1";
            for (int j = 1; j < gridWidth-1; j++)
            {
                if (i < gridHeight / 2 - 3 || i > gridHeight / 2 + 3 || j < gridWidth / 2 - 3 || j > gridWidth / 2 + 3)
                    collisionMap += "1";
                else if (i == gridHeight / 2 && j == gridWidth / 2)
                    collisionMap += "P";
                else
                    collisionMap += "0";
            }
            if(i < gridHeight - 1) collisionMap += "1\n";
        }
        return collisionMap;
    }

    public static void ReadCollisionMap(string collisionMap, MapData mapData)
    {
        mapData.baseTilemap.ClearAllTiles();
        if (mapData.carsTilemap != null) mapData.carsTilemap.ClearAllTiles();
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.Split("\n"[0]));

        List<Vector3Int> dirtPositions = new List<Vector3Int>();
        List<Vector3Int> pathPositions = new List<Vector3Int>();
        List<Vector3Int> borderPositions = new List<Vector3Int>();
        List<RuleTile> dirtTiles = new List<RuleTile>();
        List<RuleTile> pathTiles = new List<RuleTile>();
        List<RuleTile> borderTiles = new List<RuleTile>();
        for (int i = -borderThickness; i < eachLine.Count + borderThickness; i++)
        {
            for (int j = -borderThickness; j < eachLine[0].Length + borderThickness; j++)
            {
                if (i >= 0 && i < eachLine.Count && j >= 0 && j < eachLine[(int)Mathf.Max(0, Mathf.Min(eachLine.Count-1, i))].Length)
                {
                    if (eachLine[i][j] == '1')
                    {
                        dirtPositions.Add(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0));
                        dirtTiles.Add(mapData.GetDirtTile());
                        //mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.GetDirtTile());
                    }
                    else if (eachLine[i][j] == '0')
                    {
                        pathPositions.Add(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0));
                        pathTiles.Add(mapData.GetPathTile());
                        //mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.GetPathTile());
                    }
                    else if (eachLine[i][j] == 'E')
                    {
                        pathPositions.Add(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0));
                        pathTiles.Add(mapData.GetPathTile());
                        //mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.GetPathTile());
                        mapData.enemyPositions.Enqueue(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0));
                        if (mapData.carsTilemap != null) mapData.carsTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.enemyTile);
                    }
                    else if (eachLine[i][j] == 'P')
                    {
                        pathPositions.Add(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0));
                        pathTiles.Add(mapData.GetPathTile());
                        //mapData.baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.GetPathTile());
                        mapData.SetPlayerPosition(j - eachLine[i].Length / 2, -i + eachLine.Count / 2);
                        if (mapData.carsTilemap != null) mapData.carsTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), mapData.playerTile);
                    }
                } else
                {
                    borderPositions.Add(new Vector3Int(j - eachLine[0].Length / 2, -i + eachLine.Count / 2, 0));
                    borderTiles.Add(mapData.GetBorderTile());
                }
            }
        }
        
        mapData.baseTilemap.SetTiles(dirtPositions.ToArray(), dirtTiles.ToArray());
        mapData.baseTilemap.SetTiles(pathPositions.ToArray(), pathTiles.ToArray());
        mapData.baseTilemap.SetTiles(borderPositions.ToArray(), borderTiles.ToArray());
        mapData.baseTilemap.CompressBounds();
    }

    public static string CreateCollisionMap(MapData mapData)
    {
        string col = "";
        for (int j = mapData.baseTilemap.cellBounds.yMax - 1 - borderThickness; j >= mapData.baseTilemap.cellBounds.yMin + borderThickness; j--)
        {
            for (int i = mapData.baseTilemap.cellBounds.xMin + borderThickness; i < mapData.baseTilemap.cellBounds.xMax - borderThickness; i++)
            {
                if (!mapData.baseTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals(mapData.GetBorderTile().name))
                {
                    if (mapData.carsTilemap.HasTile(new Vector3Int(i, j, 0)))
                        col += mapData.carsTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals(mapData.enemyTile.name) ? "E" : "P";
                    else
                        col += mapData.baseTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals(mapData.GetPathTile().name) ? "0" : "1";
                }
            }
            if (j > mapData.baseTilemap.cellBounds.yMin + borderThickness) col += "\n";
        }
        return col;
    }

}