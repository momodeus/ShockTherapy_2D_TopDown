using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonDownMove : MonoBehaviour, IPointerDownHandler
{

    public PlayerMovement player;
    public GridMovement.Direction direction;
    // Start is called before the first frame update


    public void OnPointerDown(PointerEventData eventData)
    {
        player.RequestNewMove(direction);
    }
}
