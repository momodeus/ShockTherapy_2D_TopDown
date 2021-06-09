using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualDPad : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private Touch theTouch;
    private Vector2 touchStartPosition, touchEndPosition;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Began)
            {
                touchStartPosition = theTouch.position;
            }

            else if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
            {
                touchEndPosition = theTouch.position;
                float x = touchEndPosition.x - touchStartPosition.x;
                float y = touchEndPosition.y - touchStartPosition.y;
                if (x * x + y * y > 0.0) {
                    if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0)
                    {
                        //tapped
                    }

                    else if (Mathf.Abs(x) > Mathf.Abs(y))
                    {
                        playerMovement.RequestNewMove(x > 0 ? GridMovement.EAST : GridMovement.WEST);
                    }

                    else
                    {
                        playerMovement.RequestNewMove(y > 0 ? GridMovement.NORTH : GridMovement.SOUTH);
                    }
                }
            }
        }
    }
}
