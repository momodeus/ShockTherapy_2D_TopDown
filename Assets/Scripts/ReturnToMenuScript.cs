using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuScript : MonoBehaviour
{
    public MapData mapData;
    public void ReturnToMainMenu()
    {
        if (mapData.hasPlayer)
        {
            CollisionGenerator.CreateAndSaveCollisionMap(mapData);
            SceneManager.LoadScene("MainMenu");
        } else
        {

        }
    }
}
