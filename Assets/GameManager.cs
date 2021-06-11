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
        UpdateScore(flagScore);
        if(flagsRemaining == 0)
        {
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
    public int GetScore() { return score; }
    public StateType GetState() { return state; }

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

    public bool StartGame(int startFlags, float startFuel)
    {
        if (state != StateType.MAINMENU) return false;
        state = StateType.GAMEPLAYING;
        flagsRemaining = startFlags;
        fuelRemaining = startFuel;
        gameStartTime = Time.unscaledTimeAsDouble;
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameStarted();
        }
        return true;
    }

    public void GameLost()
    {
        if (state != StateType.GAMEPLAYING) return;
        state = StateType.GAMELOST;

        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameLost();
        }
    }
    public void GameWon()
    {
        if (state != StateType.GAMEPLAYING) return;
        state = StateType.GAMEWIN;
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameWon();
        }
    }
}

interface GameManagerListener
{
    void OnGameLost();
    void OnGameStarted();
    void OnGameWon();
    void OnScoreChanged();
}
