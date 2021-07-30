using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            uint fourletters = 0U;
            for (int x = 0; x < 36; x++)
            {
                if (x >= line.Length)
                {
                    fourletters = fourletters << 1;
                }
                else
                {
                    fourletters = fourletters << 1;
                    fourletters += ((line[x] == '1') ? 1U : 0U);
                    if (line[x] == 'P') playerPosition = new Vector2Int(x, lines.Length - invY); //TODO: proper pos
                    else if (line[x] == 'F') fnemyPositions.Add(new Vector2Int(x, lines.Length - invY)); //todo: proper pos
                    else if (line[x] == 'E') enemyPositions.Add(new Vector2Int(x, lines.Length - invY)); //todo: proper pos
                }
                if (x % 4 == 3)
                {
                    output += (char)('0' + fourletters); //numbers 0-15 correspond to 0,1,2,3,4,5,6,7,8,9,:,;,<,=,>,?
                    fourletters = 0U;
                }
            }
            output += '\n';
        }
        string proxy = "";
        proxy += "P|" + playerPosition.x + "|" + playerPosition.y + "\n";
        foreach (Vector2Int e in enemyPositions)
        {
            proxy += "E|" + e.x + "|" + e.y + "\n";
        }
        foreach (Vector2Int f in fnemyPositions)
        {
            proxy += "F|" + f.x + "|" + f.y + "\n";
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
        int xpos = 0; //makes sure we get rid of the two garbage zeroes at the end
        string[] lines = compressed.Split('\n');
        int startOfCollision = -1;
        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];
            if (line[0] == 'P' || line[0] == 'E' || line[0] == 'F')
            {
                string[] vals = line.Split('|');

            }
            else
            {
                if (startOfCollision < 0) startOfCollision = y;
            }
        }
        foreach (char c in compressed)
        {
            if (c == '\n')
            {
                result += '\n';
                xpos = 0;
            }
            else
            {
                int word = c - '0'; //so now chars '0'-'?' map to ints 0-15
                for (int i = 0; i < 4; i++)
                {
                    if (xpos <= 33) result += ((word) & (8 >> i)) > 0 ? '1' : '0';
                    xpos++;
                }
            }
        }
        return result;
    }
}
