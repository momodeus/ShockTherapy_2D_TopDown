using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float gridSize = 1; //should be ~0.48
    public float timeToMove = 0.2f;
    public int startGridX = 15, startGridY = 27;
    public float startX, startY;
    public TextAsset collisionMap;

    //direction constants (don't want to see in inspector, but valuable to other places)
    [HideInInspector]
    public const int NORTH = 0;
    [HideInInspector]
    public const int SOUTH = 2;
    [HideInInspector]
    public const int EAST = 3;
    [HideInInspector]
    public const int WEST = 1;
    [HideInInspector]
    public const int NONE = -1;

    private int heading = NORTH;
    private int gridX, gridY;
    private bool[,] collisions;
    private bool[] canMove = new bool[4] { false, false, false, false };
    private bool isMoving = false;
    private Vector3 origPos, targetPos;
    private Quaternion origHeading, targetHeading;
    private int queuedDirection = -1;
    void Start()
    {
        //reading collision file and setting up collision matrix
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.text.Split("\n"[0]));
        gridX = startGridX;
        gridY = startGridY;
        
        collisions = new bool[eachLine[0].Length, eachLine.Count]; 
        for (int i = 0; i < eachLine.Count; i++)
        {
            Debug.Log("line " + i + ": " + eachLine[i]);
            for(int j = 0; j < eachLine[i].Length; j++)
            {
                collisions[j, eachLine.Count - (i+1)] = eachLine[i][j] == '0'; //inserting lines in inverse order, to keep +y facing North. 
            }
        }
        transform.position = new Vector3(gridSize * (gridX - 0.5f * collisions.GetLength(0)) + startX, gridSize * (gridY - 0.5f * collisions.GetLength(1)) + startY, 0);
        UpdateCanMove();
    }
    // Update is called once per frame
    void Update()
    {
        if (queuedDirection == NORTH && !isMoving && heading != SOUTH && canMove[NORTH])
        {
            StartCoroutine(MovePlayer(NORTH));
            queuedDirection = NONE;
        }
        else if (queuedDirection == WEST && !isMoving && heading != EAST && canMove[WEST])
        {
            StartCoroutine(MovePlayer(WEST));
            queuedDirection = NONE;
        }
        else if (queuedDirection == SOUTH && !isMoving && heading != NORTH && canMove[SOUTH])
        {
            StartCoroutine(MovePlayer(SOUTH));
            queuedDirection = NONE;
        }
        else if (queuedDirection == EAST && !isMoving && heading != WEST && canMove[EAST])
        {
            StartCoroutine(MovePlayer(EAST));
            queuedDirection = NONE;
        }
        else if (!isMoving && canMove[heading])
        {
            StartCoroutine(MovePlayer(heading));
        }
        /* keyboard control
        if (Input.GetKey(KeyCode.W) && !isMoving && heading != SOUTH && canMove[NORTH])
        {
            StartCoroutine(MovePlayer(NORTH));
        } else if (Input.GetKey(KeyCode.A) && !isMoving && heading != EAST && canMove[WEST])
        {
            StartCoroutine(MovePlayer(WEST));
        } else if (Input.GetKey(KeyCode.S) && !isMoving && heading != NORTH && canMove[SOUTH])
        {
            StartCoroutine(MovePlayer(SOUTH));
        } else if (Input.GetKey(KeyCode.D) && !isMoving && heading != WEST && canMove[EAST])
        { 
            StartCoroutine(MovePlayer(EAST));
        } else if(!isMoving && canMove[heading])
        {
            StartCoroutine(MovePlayer(heading));
        }
        */

    }


    public void RequestNewMove(int newHeading)
    {
        queuedDirection = newHeading;
    }

    private void UpdateCanMove()
    {
        canMove[NORTH] = collisions[gridX, gridY + 1];
        canMove[EAST] = collisions[gridX + 1, gridY];
        canMove[WEST] = collisions[gridX - 1, gridY];
        canMove[SOUTH] = collisions[gridX, gridY - 1];
    }

    private IEnumerator MovePlayer(int newHeading)
    {
        isMoving = true;
        Vector3 direction;

        //updating direction and grid position
        switch (newHeading)
        {
            case NORTH:
                direction = Vector3.up;
                gridY++;
                break;
            case SOUTH:
                direction = Vector3.down;
                gridY--;
                break;
            case EAST:
                direction = Vector3.right;
                gridX++;
                break;
            case WEST:
                direction = Vector3.left;
                gridX--;
                break;
            default:
                direction = Vector3.up;
                gridY++;
                break;
        }
        UpdateCanMove();
        Debug.Log("Grid Pos: [" + gridX + ", " + gridY + "]");
        heading = newHeading;
        float elapsedTime = 0;
        origPos = transform.position;
        targetPos = origPos + direction * gridSize;

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
