using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for all UTVs on the map. 
/// Contains information about where it is on the grid, 
/// speed of movement, whether it can turn around. 
/// </summary>
public class UTV : MonoBehaviour, GameManagerListener
{
    public int heading = GridMovement.NORTH;
    public int gridX, gridY; //set these to start it off
    public float timeToMove = 0.2f;
    public GridMovement grid;
    public bool canTurn180 = true;
    protected bool isMoving = false;
    protected Vector3 origPos, targetPos;
    protected Quaternion origHeading, targetHeading;
    protected bool allowedToMove = true;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    /// <summary>
    /// verifies that gridX and gridY are not in an obstructed cell, 
    /// and moves object to its proper location based on gridX and gridY
    /// </summary>
    protected void Setup()
    {
        //add this as a listener to GameManager
        GameManager.Instance.AddGameManagerListener(this);
        //have to verify that gridX and gridY are good
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
        //to be implemented in player / enemy (/ ghost?) subclasses
    }

    /// <summary>
    /// Tries to move UTV in direction given
    /// </summary>
    /// <param name="newHeading">new direction to move</param>
    /// <returns>whether the move was successful</returns>
    protected bool TryMove(int newHeading)
    {
        if (!allowedToMove) return false;
        if (newHeading == GridMovement.NONE) return false; //might want this to be true, since success; however, doesn't move
        if ((newHeading % 4) != (heading + 2) % 4 && !canTurn180) return false;
        if (!isMoving && grid.CanMove(newHeading, gridX, gridY))
        {
            StartCoroutine(MoveRoutine(newHeading));
            return true;
        }
        return false;
    }

    //TODO: Figure out how to smooth out movement; it's pretty jittery at times
    /// <summary>
    /// Moves the UTV smoothly to its new location and rotates it in the proper direction
    /// </summary>
    /// <param name="newHeading"></param>
    /// <returns></returns>
    private IEnumerator MoveRoutine(int newHeading)
    {
        isMoving = true;
        Vector3 direction;

        switch(newHeading)
        {
            case GridMovement.NORTH:
                direction = Vector3.up;
                gridY++;
                break;
            case GridMovement.SOUTH:
                direction = Vector3.down;
                gridY--;
                break;
            case GridMovement.EAST:
                direction = Vector3.right;
                gridX++;
                break;
            case GridMovement.WEST:
                direction = Vector3.left;
                gridX--;
                break;
            default:
                direction = Vector3.up;
                gridY++;
                break;
        }
        heading = newHeading;

        float elapsedTime = 0; //this might be source of problems with jitter
        origPos = transform.position;
        targetPos = origPos + direction * grid.GetGridSize();

        origHeading = transform.rotation;
        targetHeading = Quaternion.Euler(0, 0, heading * 90);
        while(elapsedTime < timeToMove && allowedToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            transform.rotation = Quaternion.Slerp(origHeading, targetHeading, (elapsedTime / timeToMove));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.rotation = Quaternion.Slerp(origHeading, targetHeading, 1);
        transform.position = targetPos;

        isMoving = false;
    }

    public void OnGameLost() { allowedToMove = false; }
    public void OnGameStarted() { allowedToMove = true; }
    public void OnGameWon() { allowedToMove = false; }
    public void OnScoreChanged() { }
}
