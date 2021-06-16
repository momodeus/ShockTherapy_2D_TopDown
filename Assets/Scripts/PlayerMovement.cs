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
    public GameObject smokePrefab;
    private int queuedHeading = -1; //holds queued heading. This imrpoves responsiveness if player tries to move slightly before they're allowed to
    public int maxSmokes = 3;
    private int lastSmokeX = 0, lastSmokeY = 0;
    private bool android;

    void Start()
    {
        Setup();
        lastSmokeX = gridX;
        lastSmokeY = gridY;
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
            if(Input.GetKey(KeyCode.LeftShift) && SmokeScript.numSmokes < maxSmokes)
            {
                SpawnSmokeObject();
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

    void SpawnSmokeObject()
    {
        if(lastSmokeX != gridX || lastSmokeY != gridY)
        {
            SmokeScript sms = (Instantiate(smokePrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent(typeof(SmokeScript)) as SmokeScript);
            sms.grid = this.grid;
            sms.SetGridPosition(lastGridX, lastGridY);
            lastSmokeX = gridX;
            lastSmokeY = gridY;
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
