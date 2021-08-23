using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDemoController : MonoBehaviour
{
    public BotMovement bot;
    public MapData mapData;
    // Start is called before the first frame update
    void Awake()
    {
        LoadMap();
        bot.SetGridPosition(GridMovement.GetPlayerSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMap()
    {
        GridMovement.LoadMap(GameManager.Instance.UserMadeMap ?
                                GameManager.Instance.CollisionMap :
                                CollisionGenerator.BlankCollisionMap());
        CollisionGenerator.ReadCollisionMap(GameManager.Instance.UserMadeMap ?
                                GameManager.Instance.CollisionMap :
                                CollisionGenerator.BlankCollisionMap(), mapData);
    }
}
