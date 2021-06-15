using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager instance = new GameManager();

    private int flagsRemaining;
    private int livesRemaining;
    private int score;
    private float fuelRemaining;
    private float startFuel;
    private double gameStartTime;
    private StateType state = StateType.DEFAULT;
    private List<GameManagerListener> listeners = new List<GameManagerListener>();
    private GameManager() { }

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
    public int GetFlagsRemaining() { return flagsRemaining; }
    public int GetLivesRemaining() { return livesRemaining; }
    public float GetFuelRemaining() { return fuelRemaining; }
    public int GetScore() { return score; }
    public StateType GetState() { return state; }

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
        UpdateScore(flagScore + (int)(flagScore * Easing(GetPercentFuelRemaining())) / 100 * 100);
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
        Debug.Log("You Won!");
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameWon();
        }
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
