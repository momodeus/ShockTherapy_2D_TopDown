using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalGameManager : MonoBehaviour, GameManagerListener
{
    public GameObject winSplash;
    public GameObject loseSplash;
    public GameObject pauseSplash;
    public Text scoreText;
    public RectTransform fuelBar;
    public Button pauseButton;
    public float startFuel = 100.0f;
    public int numFlagsToSpawn = 10;
    public static int flagScore = 1000;
    public float fuelBarInitialWidth; //figure out a better way, but for now it's ok
    // Start is called before the first frame update
    void Awake()
    {
        GameManager.Instance.StartGame(numFlagsToSpawn, startFuel);
        GameManager.Instance.AddGameManagerListener(this);
        pauseSplash.SetActive(false);
        winSplash.SetActive(false);
        loseSplash.SetActive(false);
        //scoreText.text = "SCORE:\n" + GameManager.Instance.GetScore();
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
        SceneManager.LoadScene("MainMenu");
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