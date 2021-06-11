using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The CameraController script allows the camera to follow a 
/// selected GameObject around. 
/// 
/// The "offset" Vector3 is necessary if an offset is wanted from the object;
/// for this project, no offset is wanted, so it is commented out 
/// </summary>
public class CameraController : MonoBehaviour
{
    public GameObject player;
    //private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        //transform.position = offset + player.transform.position;
    }
}
