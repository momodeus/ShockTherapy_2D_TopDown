using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: RESTRUCTURING SO THAT GRIDMOVEMENT IS GLOBAL AND PLAYERMOVEMENT / ENEMYMOVEMENT IS INSTANTIATED
//      WANT TO FIGURE OUT HOW TO MAKE ENEMIES MOVE RANDOMLY AT FIRST, THEN MOVE TOWARDS PLAYER. 
public class PlayerMovement : UTV
{
    
    private int queuedHeading = -1;


    private bool android;

    void Start()
    {
        SetupPosition();
#if UNITY_EDITOR
android = false;
#elif UNITY_ANDROID
android = true;
#endif
    }
    // Update is called once per frame
    void Update()
    {
        if(android) { 
            if (queuedHeading == GridMovement.NONE)
            {
                TryMove(heading);
            } else
            {
                if (TryMove(queuedHeading))
                {
                    queuedHeading = GridMovement.NONE;
                } else
                {
                    TryMove(heading);
                }
            }
        } else {

            bool moved = false;
 
            if(Input.GetKey(KeyCode.W))
            {
                moved = TryMove(GridMovement.NORTH);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                moved = TryMove(GridMovement.WEST);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moved = TryMove(GridMovement.SOUTH);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moved = TryMove(GridMovement.EAST);
            }
            if(!moved)
            {
                TryMove(heading);
            }
        }
    }


    public void RequestNewMove(int newHeading)
    {
        queuedHeading = newHeading;
    }

    
}
