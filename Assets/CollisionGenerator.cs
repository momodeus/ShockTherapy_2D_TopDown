using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TextAsset collisionMap;
    public RuleTile dirtTile, pathTile, borderTile;
    
    // Start is called before the first frame update
    void Start()
    {
        ReadCollisionMap();
    }

    private void ReadCollisionMap()
    {
        tilemap.ClearAllTiles();
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.text.Split("\n"[0]));
        for (int i = -5; i < eachLine.Count + 5; i++)
        {
            for (int j = -5; j < eachLine[0].Length + 5; j++)
            {
                tilemap.SetTile(new Vector3Int(j - eachLine[0].Length / 2, -i + eachLine.Count / 2, 0), borderTile);
                if (i >= 0 && i < eachLine.Count && j >= 0 && j < eachLine[(int)Mathf.Max(0, Mathf.Min(eachLine.Count-1, i))].Length)
                {
                    if (eachLine[i][j] == '1')
                        tilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), dirtTile);
                    if (eachLine[i][j] == '0' || eachLine[i][j] == 'E' || eachLine[i][j] == 'P')
                        tilemap.SetTile(new Vector3Int(j - eachLine[i].Length / 2, -i + eachLine.Count / 2, 0), pathTile);
                }
            }
        }
        
        tilemap.CompressBounds();

    }
    private void CreateCollisionMap()
    {
        tilemap.CompressBounds();
        Debug.Log("width: " + (tilemap.cellBounds.xMax - tilemap.cellBounds.xMin) +
            "\nheight: " + (tilemap.cellBounds.yMax - tilemap.cellBounds.yMin));

        string col = "";
        for (int j = tilemap.cellBounds.yMax - 1; j >= tilemap.cellBounds.yMin; j--)
        {
            for (int i = tilemap.cellBounds.xMin; i < tilemap.cellBounds.xMax; i++)
            {
                col += tilemap.GetTile(new Vector3Int(i, j, 0)).name.Equals("Path Tile") ? "0" : "1";
            }
            if (j > tilemap.cellBounds.yMin) col += "\n";
        }
        Debug.Log(col);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}