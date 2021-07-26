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
    public UTV player;
    public Vector2 offset = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + offset.x,
            player.transform.position.y + offset.y, transform.position.z);
        if (player.IsMoving())
        {
            transform.position += new Vector3(0.03f * Random.Range(-player.roughness, player.roughness),
            0.03f * Random.Range(-player.roughness, player.roughness), 0);
        }
    }
}
