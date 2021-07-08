using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controller for player movement. Is currently set up to handle both unity editor and android mobile implementations.
/// On computer, WASD controls move player. 
/// On android, swipes move player. 
/// </summary>
public class PlayerMovement : UTV
{
    public float fuelUsedPerMove = 0.1f;
    public GameObject smokePrefab;
    public Sprite[] cars = new Sprite[9];
    public AudioClip pickupSound, crashSound, winSound;
    public Camera mainCamera;
    private int queuedHeading = -1; //holds queued heading. This imrpoves responsiveness if player tries to move slightly before they're allowed to
    private int maxSmokes = 3;
    private int lastSmokeX = 0, lastSmokeY = 0;
    private bool shouldSmoke = false;

    void Start()
    {
        gridX = GridMovement.GetPlayerSpawn().x;
        gridY = GridMovement.GetPlayerSpawn().y;
        Setup();
        lastSmokeX = gridX;
        lastSmokeY = gridY;
        (this.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).sprite = cars[GameManager.Instance.GetSelectedCar()];
        timeToMove = 0.20f - GameManager.Instance.GetSelectedCar() * 0.005f;
    }
    // Update is called once per frame
    void Update()
    {
        if (!allowedToMove) return;
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

 
        if(Input.GetKey(KeyCode.W))
        {
            RequestNewMove(GridMovement.NORTH);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            RequestNewMove(GridMovement.WEST);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            RequestNewMove(GridMovement.SOUTH);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RequestNewMove(GridMovement.EAST);
        }


        if(shouldSmoke && SmokeScript.numSmokes < maxSmokes)
        {
            SpawnSmokeObject();
        }
    }

    void SpawnSmokeObject()
    {
        if(lastSmokeX != gridX || lastSmokeY != gridY)
        {
            SmokeScript sms = (Instantiate(smokePrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent(typeof(SmokeScript)) as SmokeScript);
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
            GameManager.Instance.PickupFlag(GameManager.flagScore);
            if (GameManager.Instance.GetFlagsRemaining() == 0) AudioSource.PlayClipAtPoint(winSound, mainCamera.transform.position);
            other.gameObject.SetActive(false);
            AudioSource.PlayClipAtPoint(pickupSound, mainCamera.transform.position);

        } else if (other.gameObject.CompareTag("Enemy"))
        {
            AudioSource.PlayClipAtPoint(crashSound, mainCamera.transform.position);
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

    public void SwipeRequestNewMove(int newHeading)
    {
        if(GameManager.Instance.IsSwipeControls())
        {
            Debug.Log("swipe requested, got");
            RequestNewMove(newHeading);
        }
    }

    public void ButtonRequestNewMove(int newHeading)
    {
        if(!GameManager.Instance.IsSwipeControls())
        {
            Debug.Log("button requested, got");
            RequestNewMove(newHeading);
        }
    }
    public void SetShouldSmoke(bool s)
    {
        shouldSmoke = s;
    }
}
