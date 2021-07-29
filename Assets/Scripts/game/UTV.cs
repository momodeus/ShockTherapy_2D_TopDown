using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for all UTVs on the map. 
/// Contains information about where it is on the grid, 
/// speed of movement, whether it can turn around. 
/// </summary>
public class UTV : GridObject, GameManagerListener
{
    [Header("Movement")]
    [Min(0.001f)]public float timeToMove = 0.2f;
    [Min(0.001f)]public float timeToTurn = 0.2f;
    [HideInInspector] public float roughness = 0f; //between 0 and 1, rougness introduces camera shake and inconsistient movement speed. 
    //[Min(0.001f)]public float acceleration = 0.2f; //TODO
    public bool canTurn180 = true;
    protected bool isMoving = false;
    protected Vector3 origPos, targetPos;
    protected Quaternion origHeading, targetHeading;
    [HideInInspector]
    public bool allowedToMove = true;
    [HideInInspector]
    public GridMovement.Direction heading = GridMovement.NORTH;
    // Start is called before the first frame update
    void Start()
    {
        //add this as a listener to GameManager
        GameManager.Instance.AddGameManagerListener(this);
        //then do default setup
        Setup();
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
    protected bool TryMove(GridMovement.Direction newHeading)
    {
        if (!allowedToMove) return false;
        if (newHeading == GridMovement.NONE) return false; //might want this to be true, since success; however, doesn't move
        if (((int)newHeading % 4) != ((int)heading + 2) % 4 && !canTurn180) return false;
        if (!isMoving && GridMovement.CanMove(newHeading, gridX, gridY))
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
    private IEnumerator MoveRoutine(GridMovement.Direction newHeading)
    {
        isMoving = true;
        Vector3 direction;
        lastGridX = gridX;
        lastGridY = gridY;
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

        float elapsedTime = 0;
        float time = heading == newHeading ? timeToMove : timeToTurn;
        heading = newHeading;
        origPos = transform.position;
        targetPos = origPos + direction;

        origHeading = transform.rotation;
        targetHeading = Quaternion.Euler(0, 0, (int)(heading) * 90);
        while(elapsedTime < time && allowedToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / time));
            transform.rotation = Quaternion.Slerp(origHeading, targetHeading, (elapsedTime / time));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.rotation = targetHeading;
        transform.position = targetPos;

        isMoving = false;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void OnGameLost() { allowedToMove = false; }
    public void OnGameStarted() { allowedToMove = true; }
    public void OnGameWon() { allowedToMove = false; }
    public void OnScoreChanged() { }
}
