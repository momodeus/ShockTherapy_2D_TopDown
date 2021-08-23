using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class dPadSpriteSwitcher : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Sprite noneSprite, upSprite, leftSprite, rightSprite, downSprite;

    private GridMovement.Direction prev = GridMovement.NONE;
    private Image dPad;
    // Start is called before the first frame update
    void Start()
    {
        dPad = GetComponent<Image>();
        prev = playerMovement.queuedHeading;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.queuedHeading != prev)
        {
            prev = playerMovement.queuedHeading;
            switch (prev)
            {
                case GridMovement.NONE:
                    dPad.sprite = noneSprite;
                    break;
                case GridMovement.NORTH:
                    dPad.sprite = upSprite;
                    break;
                case GridMovement.SOUTH:
                    dPad.sprite = downSprite;
                    break;
                case GridMovement.EAST:
                    dPad.sprite = rightSprite;
                    break;
                case GridMovement.WEST:
                    dPad.sprite = leftSprite;
                    break;
            }
        }
    }
}
