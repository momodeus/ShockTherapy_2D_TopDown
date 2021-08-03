using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;

public class Compression
{
    /// <summary>
    /// WIP: DO NOT USE
    /// </summary>
    /// <param name="uncompressed"></param>
    /// <returns></returns>
    public static string CompressCollisionMap(string uncompressed)
    {
        string[] lines = uncompressed.Split('\n');
        string output = "";
        List<Vector2Int> enemyPositions = new List<Vector2Int>();
        List<Vector2Int> fnemyPositions = new List<Vector2Int>(); //extra hard enemy positions
        Vector2Int playerPosition = new Vector2Int(0, 0);
        for (int invY = 0; invY < lines.Length; invY++)
        {
            string line = lines[invY];
            if (line.Length < 2) continue;
            uint sixletters = 0U;
            for (int x = 0; x < 36; x++)
            {
                if (x >= line.Length)
                {
                    sixletters = sixletters << 1;
                }
                else
                {
                    sixletters = sixletters << 1;
                    sixletters += ((line[x] == '1') ? 1U : 0U);
                    if (line[x] == 'P') playerPosition = new Vector2Int(x, invY); //TODO: proper pos
                    else if (line[x] == 'F') fnemyPositions.Add(new Vector2Int(x, invY)); //todo: proper pos
                    else if (line[x] == 'E') enemyPositions.Add(new Vector2Int(x, invY)); //todo: proper pos
                }
                if (x % 6 == 5)
                {
                    output += (char)('!' + sixletters); //numbers 0-64 correspond to '!' through 'a'
                    sixletters = 0U;
                }
            }
            if (invY < lines.Length - 1) output += '\n';
        }
        string proxy = "";
        proxy += "p|" + playerPosition.x + "|" + playerPosition.y + "\n";
        foreach (Vector2Int e in enemyPositions)
        {
            proxy += "e|" + e.x + "|" + e.y + "\n";
        }
        foreach (Vector2Int f in fnemyPositions)
        {
            proxy += "f|" + f.x + "|" + f.y + "\n";
        }
        output = proxy + output;
        return output;
    }

    /// <summary>
    /// WIP: DO NOT USE
    /// </summary>
    /// <param name="compressed"></param>
    /// <returns></returns>
    public static string DecompressCollisionMap(string compressed)
    {
        string result = "";
        string[] lines = compressed.Split('\n');
        int startOfCollision = -1; //will be -1 until set on first run of reading collision map
        List<Vector2Int> enemyPositions = new List<Vector2Int>();
        List<Vector2Int> fnemyPositions = new List<Vector2Int>();
        Vector2Int playerPosition = new Vector2Int(1,1);
        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];
            if (line.Length < 2) continue;
            if (line[0] == 'p' || line[0] == 'e' || line[0] == 'f')
            {
                string[] vals = line.Split('|');
                Debug.Log(vals.Length);
                switch (vals[0][0])
                {
                    case 'P':
                        playerPosition = new Vector2Int(Int32.Parse(vals[1]), Int32.Parse(vals[2]));
                        break;
                    case 'E':
                        enemyPositions.Add(new Vector2Int(Int32.Parse(vals[1]), Int32.Parse(vals[2])));
                        break;
                    case 'F':
                        fnemyPositions.Add(new Vector2Int(Int32.Parse(vals[1]), Int32.Parse(vals[2])));
                        break;
                }
            }
            else
            {
                if (startOfCollision < 0) startOfCollision = y;
                int x = 0;
                foreach(char c in line)
                {
                    int word = c - '!';
                    for(int i = 0; i < 6; i++)
                    {
                        if (x == playerPosition.x && (y - startOfCollision) == playerPosition.y)
                            result += 'P';
                        else if(enemyPositions.Contains(new Vector2Int(x, y - startOfCollision)))
                        {
                            result += 'E';
                        } else if(fnemyPositions.Contains(new Vector2Int(x, y - startOfCollision)))
                        {
                            result += 'F';
                        }
                        else if (x <= 33)
                            result += ((word) & (32 >> i)) > 0 ? '1' : '0';
                        x++;
                    }
                }

                if (y < lines.Length - 1) result += '\n';
            }
        }
        return result;
    }
}
