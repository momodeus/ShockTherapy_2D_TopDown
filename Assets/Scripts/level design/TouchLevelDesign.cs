using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TouchLevelDesign : MonoBehaviour
{
    public MapData mapData;
    public Image dirtButton, pathButton, enemyButton, playerButton;
    public Toggle panToggle;
    GraphicRaycaster raycaster;
    public GameObject saveButton;
    public GameObject needsPlayerText;
    public Dropdown enemiesDropdown;
    private int enemiesCount = 1;

    private Touch theTouch;
    private int selectedTile = 0;

    private void Start()
    {
        if(GameManager.Instance.UserMadeMap)
        {
            CollisionGenerator.ReadCollisionMap(GameManager.Instance.CollisionMap, mapData);
            saveButton.SetActive(mapData.hasPlayer);
            needsPlayerText.SetActive(!mapData.hasPlayer);
        } else
        {
            CollisionGenerator.CreateFromScratch(mapData);
        }
        
    }

    void Update()
    {
        if(!panToggle.isOn) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if(Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(theTouch.fingerId)) return;
            if (theTouch.phase == TouchPhase.Began && selectedTile > 1)
            {
                //convert touch position to tile coordinate
                Ray ray = Camera.main.ScreenPointToRay(theTouch.position);
                Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = mapData.baseTilemap.WorldToCell(worldpoint);
                PlaceTile(position);
            } else if (selectedTile < 2 && theTouch.phase == TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(theTouch.position);
                Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = mapData.baseTilemap.WorldToCell(worldpoint);
                PlaceTile(position);
            }
        }
        if(selectedTile > 1 && Input.GetMouseButtonDown(0))
        {
            //convert click position to tile coordinate
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = mapData.baseTilemap.WorldToCell(worldpoint);
            PlaceTile(position);
        } else if(selectedTile < 2 && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldpoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = mapData.baseTilemap.WorldToCell(worldpoint);
            PlaceTile(position);
        }
    }

    public void SetEnemiesCount()
    {
        enemiesCount = Mathf.Min(9, Mathf.Max(enemiesDropdown.value+1, 0));
        mapData.CullEnemies(enemiesCount);
    }
    private void PlaceTile(Vector3Int position)
    {
        TileBase tb = mapData.baseTilemap.GetTile(position);
        if (tb == null) mapData.baseTilemap.SetTile(position, mapData.GetDirtTile());
        tb = mapData.GetDirtTile();
        if (tb.name.Equals(mapData.GetBorderTile().name)) return;
        //print(mapData.baseTilemap.GetTile(position).name);
        switch (selectedTile)
        {
            case 0: //placing dirt
                mapData.baseTilemap.SetTile(position, mapData.GetDirtTile());
                mapData.carsTilemap.SetTile(position, null);
                break;
            case 1: //placing path
                mapData.baseTilemap.SetTile(position, mapData.GetPathTile());
                break;
            case 2: //placing enemy
                if (mapData.baseTilemap.GetTile(position).name.Equals(mapData.GetPathTile().name) && !position.Equals(mapData.playerPosition))
                {
                    mapData.carsTilemap.SetTile(position, mapData.enemyTile);
                    mapData.enemyPositions.Enqueue(position);
                    mapData.CullEnemies(enemiesCount);
                }
                break;
            case 3: //placing player
                if (mapData.baseTilemap.GetTile(position).name.Equals(mapData.GetPathTile().name))
                {
                    mapData.carsTilemap.SetTile(mapData.playerPosition, null);
                    mapData.carsTilemap.SetTile(position, mapData.playerTile);
                    mapData.playerPosition = position;
                    mapData.hasPlayer = true;
                }
                break;
            default:
                break;
        }
        if (mapData.carsTilemap.ContainsTile(mapData.playerTile))
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
    public void SetActiveTile(int i)
    {
        dirtButton.color = i == 0 ? Color.green : Color.clear;
        pathButton.color = i == 1 ? Color.green : Color.clear;
        enemyButton.color = i == 2 ? Color.green : Color.clear;
        playerButton.color = i == 3 ? Color.green : Color.clear;
        selectedTile = i;
    }

}
