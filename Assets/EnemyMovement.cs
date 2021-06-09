using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : UTV
{
    private bool[] canMove = new bool[4] { false, false, false, false };
    private int canMoveByte = 0x00;
    // Start is called before the first frame update
    void Start()
    {
        SetupPosition();
        UpdateCanMove();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            int randDraw = (int)(Random.value * 16);
            int iter = 0;
            while (!canMove[randDraw % 4] && iter < 16)
            {
                randDraw = (int)(Random.value * 16);
                iter++;
            }
            TryMove(randDraw%4);
            /*
            for(int i = 0; i < canMove.Length; i++)
            {
                if(canMove[i])
                {
                    TryMove(i);
                    break;
                }
            }
            */
            UpdateCanMove();
        }
    }

    private void UpdateCanMove()
    {
        canMoveByte = 0x00;
        canMoveByte |= grid.CanMove(GridMovement.NORTH, gridX, gridY) ? 0x01 : 0x00;
        canMoveByte |= grid.CanMove(GridMovement.WEST, gridX, gridY) ? 0x02 : 0x00;
        canMoveByte |= grid.CanMove(GridMovement.SOUTH, gridX, gridY) ? 0x04 : 0x00;
        canMoveByte |= grid.CanMove(GridMovement.EAST, gridX, gridY) ? 0x08 : 0x00;
        for(int i = 0; i < 4; i++)
        {
            canMove[i] = grid.CanMove(i, gridX, gridY);
        }
        canMove[(heading + 2) % 4] = false; //can't turn around directly
    }
}
