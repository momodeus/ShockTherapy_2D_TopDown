using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class ReturnToMenuScript : MonoBehaviour
{
    public MapData mapData;
    public TransitionSceneLoader sceneLoader;
    public void ReturnToMainMenu()
    {
        if (mapData.hasPlayer)
        {
            GameManager.Instance.CollisionMap = CollisionGenerator.CreateCollisionMap(mapData);
            sceneLoader.LoadScene("MainMenu");
        } else
        {
            print("needs player");
        }
    }

}
