using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TouchLevelDesign : MonoBehaviour
{
    public Tilemap tilemap;
    public Toggle tileToggle;
    public Toggle panToggle;
    public RuleTile dirtTile, pathTile;
    private Touch theTouch;

    // Update is called once per frame
    void Update()
    {
        if(!panToggle.isOn) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);
            if(theTouch.phase == TouchPhase.Began || theTouch.phase == TouchPhase.Moved)
            {
                //convert touch position to tile coordinate
                Ray ray = Camera.main.ScreenPointToRay(theTouch.position);
                Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = tilemap.WorldToCell(worldpoint);
                tilemap.SetTile(position, tileToggle.isOn ? dirtTile : pathTile);
            }
        }
        if(Input.GetMouseButton(0))
        {
            //convert click position to tile coordinate
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = tilemap.WorldToCell(worldpoint);
            tilemap.SetTile(position, tileToggle.isOn ? dirtTile : pathTile);
        }
    }
}
