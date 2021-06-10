using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTV : MonoBehaviour
{
    public int heading = GridMovement.NORTH;
    public int gridX, gridY; //set these to start it off. TODO: verify these aren't invalid coords in Start()
    public float timeToMove = 0.2f;
    public GridMovement grid;

    protected bool isMoving = false;
    protected Vector3 origPos, targetPos;
    protected Quaternion origHeading, targetHeading;


    // Start is called before the first frame update
    void Start()
    {
        SetupPosition();
    }
    protected void SetupPosition()
    {
        transform.position = new Vector3(
            grid.gridSize * (gridX - 0.5f * grid.GetWidth()) + grid.offsetX,
            grid.gridSize * (gridY - 0.5f * grid.GetHeight()) + grid.offsetY, 0);

    }
    // Update is called once per frame
    void Update()
    {
        //to be implemented in player / enemy (/ ghost?) subclasses
    }

    /**
     * returns whether the move happened. 
     */
    protected bool TryMove(int newHeading)
    {
        if (newHeading == GridMovement.NONE) return false; //might want this to be true, since success; however, doesn't move
        if ( (newHeading % 4) != (heading + 2) % 4 && !isMoving && grid.CanMove(newHeading, gridX, gridY))
        {
            StartCoroutine(MoveRoutine(newHeading));
            return true;
        }
        return false;
    }

    //TODO: Figure out how to smooth out movement; it's pretty jittery at times
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
        while(elapsedTime < timeToMove)
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
}
