using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float gridSize = 1; //should be ~0.48
    public float timeToMove = 0.2f;
    public int startX = 15, startY = 27;
    public TextAsset collisionMap;

    //direction constants
    private const int NORTH = 0;
    private const int SOUTH = 2;
    private const int EAST = 3;
    private const int WEST = 1;

    private int heading = NORTH;
    private int gridX, gridY;
    private bool[,] collisions;
    private bool isMoving = false;
    private Vector3 origPos, targetPos;
    private Quaternion origHeading, targetHeading;

    void Start()
    {
        //reading collision file and setting up collision matrix
        List<string> eachLine = new List<string>();
        eachLine.AddRange(collisionMap.text.Split("\n"[0]));
        
        /*
         * note on indexing: in Unity, +y is up and -y is down; 
         * in collisions[x,y], +y is down and -y is up. this sucks but i'm stubborn. 
         */
        collisions = new bool[eachLine[0].Length, eachLine.Count]; 
        for (int i = 0; i < eachLine.Count; i++)
        {
            for(int j = 0; j < eachLine[i].Length; j++)
            {
                collisions[j, i] = eachLine[i][j] == '0';
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && !isMoving && heading != SOUTH)
        {
            StartCoroutine(MovePlayer(NORTH));
        } else if (Input.GetKey(KeyCode.A) && !isMoving && heading != EAST)
        {
            StartCoroutine(MovePlayer(WEST));
        } else if (Input.GetKey(KeyCode.S) && !isMoving && heading != NORTH)
        {
            StartCoroutine(MovePlayer(SOUTH));
        } else if (Input.GetKey(KeyCode.D) && !isMoving && heading != WEST)
        { 
            StartCoroutine(MovePlayer(EAST));
        } else if(!isMoving)
        {
            StartCoroutine(MovePlayer(heading));
        }

    }

    private IEnumerator MovePlayer(int newHeading)
    {
        Vector3 direction;
        switch (newHeading)
        {
            case NORTH:
                direction = Vector3.up;
                break;
            case SOUTH:
                direction = Vector3.down;
                break;
            case EAST:
                direction = Vector3.right;
                break;
            case WEST:
                direction = Vector3.left;
                break;
            default:
                direction = Vector3.up;
                break;
        }
        isMoving = true;
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
            elapsedTime += Time.deltaTime;  
            yield return null;
        }

        transform.rotation = Quaternion.Slerp(origHeading, targetHeading, 1);
        transform.position = targetPos;

        isMoving = false;
    }
}
