using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonDownMove : MonoBehaviour, IPointerDownHandler
{

    public PlayerMovement player;
    public int direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        player.RequestNewMove(direction);
    }
}
