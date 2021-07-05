using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionGenerator : MonoBehaviour
{
    public Tilemap baseTilemap;
    public Tilemap carsTilemap;
    public RuleTile dirtTile, pathTile, borderTile;
    public Tile enemyTile, playerTile;
    public int gridWidth, gridHeight;
    public GameObject saveButton;
    public GameObject needsPlayerText;
    public TouchLevelDesign tld;
    private const int borderThickness = 5;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.UserMadeMap())
        {
            ReadCollisionMap(GameManager.Instance.GetCollisionMap());
            if(carsTilemap.ContainsTile(playerTile))
            {
                saveButton.SetActive(true);
                needsPlayerText.SetActive(false);
            } else
            {
                saveButton.SetActive(false);
                needsPlayerText.SetActive(true);
            }
        }
        else
        {
            CreateFromScratch();
        }
    }

    private void CreateFromScratch()
    {
        baseTilemap.ClearAllTiles();
        carsTilemap.ClearAllTiles();
        for (int i = -borderThickness; i < gridHeight + borderThickness; i++)
        {
            for (int j = -borderThickness; j < gridWidth + borderThickness; j++)
            {
                if (i >= 0 && i < gridHeight && j >= 0 && j < gridWidth)
                {
                    baseTilemap.SetTile(new Vector3Int(j - gridWidth / 2, -i + gridHeight / 2, 0), pathTile);
                } else
                {
                    baseTilemap.SetTile(new Vector3Int(j - gridWidth / 2, -i + gridHeight / 2, 0), borderTile);
                }
            }
        }
        carsTilemap.SetTile(new Vector3Int(0, 0, 0), playerTile);
        tld.SetPlayerPosition(0, 0);
        baseTilemap.CompressBounds();
    }
    private void ReadCollisionMap(string collisionMap)
    {
        baseTilemap.ClearAllTiles();
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.Split("\n"[0]));
        for (int i = -borderThickness; i < eachLine.Count + borderThickness; i++)
        {
            for (int j = -borderThickness; j < eachLine[0].Length + borderThickness; j++)
            {
                baseTilemap.SetTile(new Vector3Int(j - eachLine[0].Length / 2, -i + eachLine.Count / 2, 0), borderTile);
                if (i >= 0 && i < eachLine.Count && j >= 0 && j < eachLine[(int)Mathf.Max(0, Mathf.Min(eachLine.Count-1, i))].Length)
                {
                    if (eachLine[i][j] == '1')
                        baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), dirtTile);
                    if (eachLine[i][j] == '0' || eachLine[i][j] == 'E' || eachLine[i][j] == 'P')
                        baseTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), pathTile);
                    if (eachLine[i][j] == 'E')
                        carsTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), enemyTile);
                    if (eachLine[i][j] == 'P')
                    {
                        tld.SetPlayerPosition(j - eachLine[i].Length / 2, -i + eachLine.Count / 2);
                        carsTilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), playerTile);
                    }
                }
            }
        }
        baseTilemap.CompressBounds();
    }
    public void CreateAndSaveCollisionMap()
    {
        //TODO: take into account the border
        baseTilemap.CompressBounds();
        Debug.Log("width: " + (baseTilemap.cellBounds.xMax - baseTilemap.cellBounds.xMin) +
            "\nheight: " + (baseTilemap.cellBounds.yMax - baseTilemap.cellBounds.yMin));

        string col = "";
        for (int j = baseTilemap.cellBounds.yMax - 1 - borderThickness; j >= baseTilemap.cellBounds.yMin + borderThickness; j--)
        {
            for (int i = baseTilemap.cellBounds.xMin + borderThickness; i < baseTilemap.cellBounds.xMax - borderThickness; i++)
            {
                if (!baseTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Border Tile"))
                {
                    if (carsTilemap.HasTile(new Vector3Int(i, j, 0)))
                        col += carsTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Enemy Tile") ? "E" : "P";
                    else
                        col += baseTilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Path Tile") ? "0" : "1";
                }
            }
            if (j > baseTilemap.cellBounds.yMin + borderThickness) col += "\n";
        }
        GameManager.Instance.SetCollisionMap(col);
    }
}