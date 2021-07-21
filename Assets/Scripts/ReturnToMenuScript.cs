using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuScript : MonoBehaviour
{
    public MapData mapData;
    public TransitionSceneLoader sceneLoader;
    public void ReturnToMainMenu()
    {
        if (mapData.hasPlayer)
        {
            CollisionGenerator.CreateAndSaveCollisionMap(mapData);
            
            sceneLoader.LoadScene("MainMenu");
        } else
        {
            print("needs player");
        }
    }
}
