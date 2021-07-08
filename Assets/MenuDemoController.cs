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
        GridMovement.LoadMap(GameManager.Instance.UserMadeMap() ? 
                                GameManager.Instance.GetCollisionMap() : 
                                CollisionGenerator.BlankCollisionMap());
        CollisionGenerator.ReadCollisionMap(GameManager.Instance.UserMadeMap() ? 
                                GameManager.Instance.GetCollisionMap() : 
                                CollisionGenerator.BlankCollisionMap(), mapData);
        bot.SetGridPosition(GridMovement.GetPlayerSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
