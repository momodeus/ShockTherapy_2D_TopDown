using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TouchLevelDesign : MonoBehaviour
{
    public Tilemap baseTilemap;
    public Tilemap carsTilemap;
    public Image dirtButton, pathButton, enemyButton, playerButton;
    public Toggle panToggle;
    public RuleTile dirtTile, pathTile;
    public Tile enemyTile, playerTile;
    public GameObject saveButton;
    public GameObject needsPlayerText;

    private Vector3Int playerLocation;
    private Touch theTouch;
    private int selectedTile = 0;

    void Update()
    {
        if(!panToggle.isOn) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);
            if (theTouch.phase == TouchPhase.Began || theTouch.phase == TouchPhase.Moved)
            {
                //convert touch position to tile coordinate
                Ray ray = Camera.main.ScreenPointToRay(theTouch.position);
                Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = baseTilemap.WorldToCell(worldpoint);
                PlaceTile(position);
            }
        }
        if(Input.GetMouseButton(0))
        {
            //convert click position to tile coordinate
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = baseTilemap.WorldToCell(worldpoint);
            PlaceTile(position);
        }
    }

    private void PlaceTile(Vector3Int position)
    {
        if (baseTilemap.GetTile(position).name.Equals("Border Tile")) return;
        switch (selectedTile)
        {
            case 0: //
                baseTilemap.SetTile(position, dirtTile);
                carsTilemap.SetTile(position, null);
                break;
            case 1:
                baseTilemap.SetTile(position, pathTile);
                break;
            case 2:
                if (baseTilemap.GetTile(position).name.Equals("Path Tile"))
                {
                    print("placing enemy");
                    carsTilemap.SetTile(position, enemyTile);
                }
                break;
            case 3:
                if (baseTilemap.GetTile(position).name.Equals("Path Tile"))
                {
                    carsTilemap.SetTile(playerLocation, null);
                    carsTilemap.SetTile(position, playerTile);
                    playerLocation = position;
                }
                break;
            default:
                break;
        }
        if (carsTilemap.ContainsTile(playerTile))
        {
            saveButton.SetActive(true);
            needsPlayerText.SetActive(false);
        }
        else
        {
            saveButton.SetActive(false);
            needsPlayerText.SetActive(true);
        }
    }
    public void SetPlayerPosition(int x, int y)
    {
        playerLocation = new Vector3Int(x, y, 0);
    }
    public void SetActiveTile(int i)
    {
        dirtButton.color = i == 0 ? Color.green : Color.clear;
        pathButton.color = i == 1 ? Color.green : Color.clear;
        enemyButton.color = i == 2 ? Color.green : Color.clear;
        playerButton.color = i == 3 ? Color.green : Color.clear;
        selectedTile = i;
    }
}
