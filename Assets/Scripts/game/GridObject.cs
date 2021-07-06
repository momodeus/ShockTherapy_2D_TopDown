using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public int gridX, gridY;
    protected int lastGridX, lastGridY;
    public GridMovement grid;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    protected void Setup()
    {
        if (!grid.IsUnBlocked(gridX, gridY))
        {
            float bestDist = float.MaxValue;
            int bx = gridX, by = gridY; //best grid x/y
            List<Vector2> validPoints = grid.GetValidPoints();
            Debug.Log(validPoints.ToString());
            for (int i = 0; i < validPoints.Count; i++)
            {
                Vector2 point = validPoints[i];
                float test = Dist2(gridX, gridY, point.x, point.y);
                if (test < bestDist)
                {
                    bx = (int)point.x;
                    by = (int)point.y;
                    bestDist = test;
                }
            }
            gridX = bx;
            gridY = by;
            lastGridX = gridX;
            lastGridY = gridY;
        }

        transform.position = grid.GetCoords(gridX, gridY);
    }

    /// <summary>
    /// Simple Euclidean distance squared
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    protected float Dist2(float x1, float y1, float x2, float y2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
