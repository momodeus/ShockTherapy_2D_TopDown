using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalGameManager : MonoBehaviour, GameManagerListener
{
    [Header("UI")]
    public GameObject winSplash;
    public GameObject loseSplash;
    public GameObject pauseSplash;

    public Text scoreText;
    public RectTransform fuelBar;
    public Button pauseButton;
    public float fuelBarInitialWidth; //figure out a better way, but for now it's ok

    public TransitionSceneLoader sceneLoader;
    [Header("Game Elements")]
    public GameObject flag;
    public GameObject specialFlag;
    public GameObject enemy;
    public PlayerMovement player;
    
    public float startFuel = 100.0f;
    private int numFlagsToSpawn = 10;
    public SpriteRenderer mapSpriteRenderer;

    [Header("Maps")]
    public MapData mapData;

    public TextAsset[] collisionMap = new TextAsset[3]; //text representation of map, with 1 being a wall and 0 being a path. 
                                                        //important: make sure there is no extra line at the end of file. 
    public Sprite[] mapImages = new Sprite[3];
    // Start is called before the first frame update
    void Awake()
    {
        GameManager.Instance.StartGame(numFlagsToSpawn, startFuel);
        GameManager.Instance.AddGameManagerListener(this);
        pauseSplash.SetActive(false);
        winSplash.SetActive(false);
        loseSplash.SetActive(false);
        LoadMap();
        SpawnEntities();
        //scoreText.text = "SCORE:\n" + GameManager.Instance.GetScore();
    }

    private void LoadMap()
    {
        if(GameManager.Instance.GetMap() == -1)
        {
            mapSpriteRenderer.gameObject.SetActive(false);
            GridMovement.LoadMap(GameManager.Instance.GetCollisionMap());
            CollisionGenerator.ReadCollisionMap(GameManager.Instance.GetCollisionMap(), mapData);
        } else
        {
            mapSpriteRenderer.sprite = mapImages[GameManager.Instance.GetMap()];
            GridMovement.LoadMap(collisionMap[GameManager.Instance.GetMap()].text);
        }
    }

    private void SpawnEntities()
    {
        //spawn flags
        List<Vector2Int> possible = GridMovement.GetValidPoints();
        if (possible.Count == 1) return;
        GameManager.Instance.SetFlagsRemaining(numFlagsToSpawn);
        int i = numFlagsToSpawn;
        while (i > 0 && possible.Count > 0)
        {
            int idx = (int)Random.Range(0, possible.Count - 0.001f);
            if (possible[idx].x == player.gridX && possible[idx].y == player.gridY) continue;
            GridObject go = (Instantiate(i == 1 ? specialFlag : flag, Vector3.zero, Quaternion.identity).GetComponent(typeof(GridObject)) as GridObject);
            go.SetGridPosition(possible[idx]);
            possible.RemoveAt(idx);
            i--;
        }

        EnemySpawner.SpawnEnemies(enemy, player);
        player.SetGridPosition(GridMovement.GetPlayerSpawn());
    }
    // Update is called once per frame
    void Update()
    {
        fuelBar.sizeDelta = new Vector2(GameManager.Instance.GetPercentFuelRemaining() * fuelBarInitialWidth, fuelBar.sizeDelta.y);
    }

    public void OnGameLost() 
    { 
        loseSplash.SetActive(true);
        winSplash.SetActive(false);
        (loseSplash.GetComponentsInChildren(typeof(Text))[1] as Text).text = "SCORE:\n" + GameManager.Instance.GetScore();
        (loseSplash.GetComponentsInChildren(typeof(Text))[3] as Text).text = "REWARD:\n$" + GameManager.Instance.CalculateLossMoney();
    }
    public void OnGameStarted() { }
    public void OnGameWon()
    {
        loseSplash.SetActive(false);
        winSplash.SetActive(true);
        (winSplash.GetComponentsInChildren(typeof(Text))[1] as Text).text = "SCORE:\n" + GameManager.Instance.GetScore();
        (winSplash.GetComponentsInChildren(typeof(Text))[3] as Text).text = "REWARD:\n$" + GameManager.Instance.CalculateWinMoney();
    }

    public void OnScoreChanged()
    {
        scoreText.text = "SCORE:\n" + GameManager.Instance.GetScore();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.ReturnToMainMenu();
        sceneLoader.LoadScene("MainMenu");
    }

    public void OnDestroy()
    {
        GameManager.Instance.ClearGameManagerListeners();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseSplash.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseSplash.SetActive(false);
    }
}
