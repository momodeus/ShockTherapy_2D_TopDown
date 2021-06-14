using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controller for player movement. Is currently set up to handle both unity editor and android mobile implementations.
/// On computer, WASD controls move player. 
/// On android, swipes move player. 
/// </summary>
public class PlayerMovement : UTV
{
    public float fuelUsedPerMove = 0.1f;
    private int queuedHeading = -1; //holds queued heading. This imrpoves responsiveness if player tries to move slightly before they're allowed to


    private bool android;

    void Start()
    {
        Setup();
#if UNITY_EDITOR
android = false;
#elif UNITY_ANDROID
android = true;
#endif
    }
    // Update is called once per frame
    void Update()
    {
        if (!allowedToMove) return;
        if(android) { 
            if (queuedHeading == GridMovement.NONE)
            {
                GameManager.Instance.UseFuel(TryMove(heading) ? fuelUsedPerMove : 0);
            } else
            {
                if (TryMove(queuedHeading))
                {
                    queuedHeading = GridMovement.NONE;
                    GameManager.Instance.UseFuel(fuelUsedPerMove);
                } else
                {
                    GameManager.Instance.UseFuel(TryMove(heading) ? fuelUsedPerMove : 0);
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
                GameManager.Instance.UseFuel(TryMove(heading) ? fuelUsedPerMove : 0);
            } else
            {
                GameManager.Instance.UseFuel(fuelUsedPerMove);
            }
        }
    }

    /// <summary>
    /// Figures out what player hit. If flag, pick it up; if enemy, die. 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Pickup"))
        {
            GameManager.Instance.PickupFlag(LocalGameManager.flagScore);
            other.gameObject.SetActive(false);

        } else if (other.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.GameLost();
        }
    }

    /// <summary>
    /// Allows queueing of next move
    /// </summary>
    /// <param name="newHeading">next move to queue</param>
    public void RequestNewMove(int newHeading)
    {
        queuedHeading = newHeading;
    }

    
}
