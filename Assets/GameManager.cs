using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager instance = new GameManager();

    private int startFlags;
    private int flagsRemaining;
    private int livesRemaining;
    private int score;
    private float fuelRemaining;
    private float startFuel;
    private double gameStartTime;
    private bool swipeControls;
    private const string swipeControlsKey = "SwipeControls";
    private int money;
    private const string moneyKey = "Money";
    public const int winBonus = 1000;
    public const int fuelBonus = 500;
    public const int flagValue = 100;
    private const float scoreMultiplier = 0.1f;
    private StateType state = StateType.DEFAULT;
    private List<GameManagerListener> listeners = new List<GameManagerListener>();
    private GameManager()
    {
        LoadData();
    }

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameManager();
            }

            return instance;
        }
    }
    public int GetFlagsRemaining() => flagsRemaining;
    public int GetLivesRemaining() => livesRemaining;
    public float GetFuelRemaining() => fuelRemaining;
    public int GetScore() => score;
    public StateType GetState() => state;
    public bool IsSwipeControls() => swipeControls;
    public int GetMoney() => money;
    public void SetControlScheme(bool isSwipe)
    {
        swipeControls = isSwipe;
        PlayerPrefs.SetInt(swipeControlsKey, swipeControls ? 1 : 0);
        Debug.Log("prefs swipeControlKey:" + PlayerPrefs.GetInt(swipeControlsKey));
    }

    public void UseFuel(float amount) {
        fuelRemaining -= amount;
        fuelRemaining = fuelRemaining < 0 ? 0 : fuelRemaining;
        if(fuelRemaining == 0)
        {
            GameLost();
        }
    }

    public void PickupFlag(int flagScore)
    {
        flagsRemaining--;
        Debug.Log("flags remaining: " + flagsRemaining);
        UpdateScore(flagScore + (int)(flagScore * Easing(GetPercentFuelRemaining())) / 10 * 10);
        if(flagsRemaining == 0)
        {
            Debug.Log("0 flags remain");
            GameWon();
        }
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        foreach (GameManagerListener gml in listeners) {
            gml.OnScoreChanged();
        }
    }

    public bool UpdateMoney(int amount)
    {
        if (money + amount < 0) return false;
        money += amount;
        PlayerPrefs.SetInt(moneyKey, money);
        return true;
    }
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(swipeControlsKey))
        {
            swipeControls = PlayerPrefs.GetInt(swipeControlsKey) > 0;
        }
        else
        {
            swipeControls = true;
            PlayerPrefs.SetInt(swipeControlsKey, 1);
        }
        if (PlayerPrefs.HasKey(moneyKey))
        {
            money = PlayerPrefs.GetInt(moneyKey);
        }
        else
        {
            money = 0;
            PlayerPrefs.SetInt(moneyKey, 0);
        }
    }
    public enum StateType
    {
        DEFAULT,
        GAMEPLAYING,
        GAMELOST,
        MAINMENU,
        OPTIONSMENU,
        GAMESTART,
        GAMEWIN
    }

    public void SetFlagsRemaining(int flg)
    {
        flagsRemaining = flg;
    }

    public bool StartGame(int startFlags, float startFuel)
    {
        //if (state != StateType.MAINMENU) return false;
        state = StateType.GAMEPLAYING;
        flagsRemaining = startFlags;
        this.startFlags = startFlags;
        this.startFuel = startFuel;
        fuelRemaining = startFuel;
        gameStartTime = Time.unscaledTimeAsDouble;
        score = 0;
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnScoreChanged();
            gml.OnGameStarted();
        }
        return true;
    }

    public void GameLost()
    {
        if (state != StateType.GAMEPLAYING) return;
        state = StateType.GAMELOST;
        UpdateMoney(CalculateLossMoney());
        Debug.Log("Lost!");
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameLost();
        }
    }
    public void GameWon()
    {
        if (state != StateType.GAMEPLAYING) return;
        state = StateType.GAMEWIN;
        UpdateMoney(CalculateWinMoney());
        Debug.Log("You Won!");
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameWon();
        }
    }

    public int CalculateWinMoney()
    {
        return (startFlags - flagsRemaining) * flagValue + 
            (GetPercentFuelRemaining() > 0.5 ? fuelBonus : 0) +
            winBonus;
    }

    public int CalculateLossMoney()
    {
        return (int)((startFlags - flagsRemaining) * flagValue * 0.5);
    }
    public void AddGameManagerListener(GameManagerListener gml)
    {
        listeners.Add(gml);
    }

    public void RemoveGameManagerListener(GameManagerListener gml)
    {
        listeners.Remove(gml);
    }

    public void ClearGameManagerListeners()
    {
        listeners.Clear();
    }

    public float GetPercentFuelRemaining()
    {
        return fuelRemaining / startFuel;
    }

    public void ReturnToMainMenu()
    {
        state = StateType.MAINMENU;
    }
    /// <summary>
    /// easing function from easings.net
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float Easing(float x)
    {
        return x * x * x * x;
    }
}

public interface GameManagerListener
{
    public void OnGameLost();
    public void OnGameStarted();
    public void OnGameWon();
    public void OnScoreChanged();
}
