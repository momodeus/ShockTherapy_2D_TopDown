using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuScript : MonoBehaviour
{
    public CollisionGenerator cg;
    public void ReturnToMainMenu()
    {
        if (cg.carsTilemap.ContainsTile(cg.playerTile))
        {
            cg.CreateAndSaveCollisionMap();
            SceneManager.LoadScene("MainMenu");
        } else
        {

        }
    }
}
