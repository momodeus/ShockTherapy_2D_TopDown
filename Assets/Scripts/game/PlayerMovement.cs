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
    [Header("Game Parameters")]
    public float fuelUsedPerMove = 0.1f;
    public GameObject smokePrefab;
    public Sprite[] cars = new Sprite[9];
    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip crashSound; 
    public AudioClip winSound;
    public AudioSource audioSource;
    public GameObject flagPickupAnimation;
    public GridMovement.Direction queuedHeading = GridMovement.NONE; //holds queued heading. This imrpoves responsiveness if player tries to move slightly before they're allowed to
    private int maxSmokes = 3;
    private int lastSmokeX = 0, lastSmokeY = 0;
    private bool shouldSmoke = false;
    private bool playing = true;

    void Start()
    {
        gridX = GridMovement.GetPlayerSpawn().x;
        gridY = GridMovement.GetPlayerSpawn().y;
        Setup();
        lastSmokeX = gridX;
        lastSmokeY = gridY;
        (this.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).sprite = cars[GameManager.Instance.SelectedCar];
        int[] stats = GameManager.Instance.GetStatValues();
        timeToMove = 0.25f - stats[0]*0.005f;
        timeToTurn = 0.25f - stats[1] * 0.005f;
        //TODO: acceleration?
        fuelUsedPerMove = 0.5f - stats[3] * 0.03f;
        roughness = 1 - stats[4]/11f;
    }
    // Update is called once per frame
    void Update()
    {
        if (!allowedToMove || GameManager.Instance.GetTimeSinceGameStart() < 3) return;
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
            if (GameManager.Instance.flagsRemaining == 0)
            {
                (Camera.main.gameObject.GetComponent(typeof(AudioSource)) as AudioSource).Pause();
                audioSource.Stop();
                audioSource.PlayOneShot(winSound, 0.7f);
                playing = false;
            }
            Instantiate(flagPickupAnimation, other.transform.position, other.transform.rotation);
            other.gameObject.SetActive(false);
            
            audioSource.PlayOneShot(pickupSound, 0.5f);

        } else if (other.gameObject.CompareTag("Special Pickup"))
        {
            GameManager.Instance.PickupSpecialFlag(GameManager.flagScore);
            if (GameManager.Instance.flagsRemaining == 0)
            {
                (Camera.main.gameObject.GetComponent(typeof(AudioSource)) as AudioSource).Pause();
                audioSource.Stop();
                audioSource.PlayOneShot(winSound, 0.7f);
                playing = false;
            }
            (Instantiate(flagPickupAnimation, other.transform.position, other.transform.rotation)).GetComponent<specialFlagScorer>().isMultiplied = true;
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(pickupSound, 0.5f);

        }
        else if (other.gameObject.CompareTag("Enemy") && playing)
        {
            (Camera.main.gameObject.GetComponent(typeof(AudioSource)) as AudioSource).Pause();
            audioSource.Stop();
            audioSource.PlayOneShot(crashSound, 0.5f);
            GameManager.Instance.GameLost();
            playing = false;
        }
    }

    /// <summary>
    /// Allows queueing of next move
    /// </summary>
    /// <param name="newHeading">next move to queue</param>
    public void RequestNewMove(GridMovement.Direction newHeading)
    {
        queuedHeading = newHeading;
        
    }

    public void SwipeRequestNewMove(GridMovement.Direction newHeading)
    {
        if(GameManager.Instance.SwipeControls)
        {
            Debug.Log("swipe requested, got");
            RequestNewMove(newHeading);
        }
    }

    public void ButtonRequestNewMove(int newHeading)
    {
        if(!GameManager.Instance.SwipeControls)
        {
            Debug.Log("button requested, got");
            RequestNewMove((GridMovement.Direction)newHeading);
        }
    }
    public void SetShouldSmoke(bool s)
    {
        shouldSmoke = s;
    }
}
