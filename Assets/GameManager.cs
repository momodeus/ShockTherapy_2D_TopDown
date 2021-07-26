using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager instance = new GameManager();

    private int startFlags;
    private int flagsRemaining;
    private int livesRemaining;
    private float fuelRemaining;
    private float startFuel;
    private float gameStartTime;
    private int map;
    private bool firstFlagGot = false;
    //options stuff
    private bool swipeControls;
    private const string swipeControlsKey = "SwipeControls";

    //money stuff
    private int money;
    private const string moneyKey = "Money";
    public const int winBonus = 50;
    public const int fuelBonus = 50;
    public const int flagValue = 10;

    //skins information
    private uint unlockedCars = 0; //the nth LSB of this int represents whether user has unlocked nth car. 
    private string unlockedCarsKey = "unlockedCars";
    private int selectedCar = 0; //0 - 8, the 9 different car types. 
    private string selectedCarKey = "selectedCar";

    private uint unlockedThemes = 0; //Works just like cars
    private string unlockedThemesKey = "unlockedThemes";
    private int selectedTheme = 0;
    private string selectedThemeKey = "selectedTheme";
    //score stuff
    public const int flagScore = 1000;
    private int score;
    private int highScore;
    private string highScoreKey = "highScore";

    //status stuff
    private StateType state = StateType.DEFAULT;
    private List<GameManagerListener> listeners = new List<GameManagerListener>();

    //custom map stuff
    private bool userMadeMap = false;
    private string collisionMap;
    private const string collisionMapKey = "collisionMap";


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
    public int GetSelectedCar() => selectedCar;
    public int GetSelectedTheme() => selectedTheme;
    public uint GetUnlockedCarsRaw() => unlockedCars;
    public uint GetUnlockedThemesRaw() => unlockedThemes;
    public int GetMap() => map;
    public float GetTimeSinceGameStart() => Time.time - gameStartTime;
    public int GetHighScore() => highScore;
    public string GetCollisionMap() => collisionMap;
    public bool UserMadeMap() => userMadeMap;

    public void SetCollisionMap(string map)
    {
        userMadeMap = true;
        collisionMap = map;
        PlayerPrefs.SetString(collisionMapKey, collisionMap);
    }

    public void SetSelectedCar(int car)
    {
        if (car >= 0 && car <= 8) 
        {
            selectedCar = car;
            PlayerPrefs.SetInt(selectedCarKey, selectedCar);
        }
    }

    public void SetSelectedTheme(int theme)
    {
        if(theme >= 0 && theme <= MapData.NUM_THEMES)
        {
            Debug.Log("selected: " + theme);

            selectedTheme = theme;
            PlayerPrefs.SetInt(selectedThemeKey, selectedTheme);
        }
    }
    public void SetMap(int map)
    {
        if (map < -1 || map > 2) return;
        this.map = map;
    }
    public void LockAllCars()
    {
        unlockedCars = 0U;
        PlayerPrefs.SetInt(unlockedCarsKey, (int)unlockedCars);
    }

    public void LockAllThemes()
    {
        unlockedThemes = 0U;
        PlayerPrefs.SetInt(unlockedThemesKey, (int)unlockedThemes);
    }
    public void UnlockCar(int car)
    {
        if (car < 0 || car > 7) return;
        unlockedCars |= 1U << car;
        PlayerPrefs.SetInt(unlockedCarsKey, (int)unlockedCars);
    }

    public void UnlockTheme(int theme)
    {
        if(theme >= 0 && theme <= MapData.NUM_THEMES)
        {
            Debug.Log("unlocking: " + theme);
            unlockedThemes |= 1U << theme;
            PlayerPrefs.SetInt(unlockedThemesKey, (int)unlockedThemes);
        }
    }
    public bool IsCarUnlocked(int car)
    {
        if (car == -1) return true; //default car
        if (car < 0 || car > 7) return false;
        return ((unlockedCars & (1U << car)) > 0U);
    }

    public bool IsThemeUnlocked(int theme)
    {
        if (theme == -1) return true; //default theme
        if (theme < 0 || theme > MapData.NUM_THEMES) return false;
        return ((unlockedThemes & (1U << theme)) > 0U);
    }
    public void SetControlScheme(bool isSwipe)
    {
        swipeControls = isSwipe;
        PlayerPrefs.SetInt(swipeControlsKey, swipeControls ? 1 : 0);
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
        firstFlagGot = true;
        flagsRemaining--;
        UpdateScore(flagScore + (int)(flagScore * Easing(GetPercentFuelRemaining())) / 10 * 10);
        if(flagsRemaining == 0)
        {
            GameWon();
        }
    }

    public void PickupSpecialFlag(int flagScore)
    {
        PickupFlag(flagScore + (firstFlagGot ? flagScore * 10 : flagScore));

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
        if(PlayerPrefs.HasKey(selectedCarKey))
            selectedCar = PlayerPrefs.GetInt(selectedCarKey);
        else
        {
            selectedCar = 0;
            PlayerPrefs.SetInt(selectedCarKey, 0);
        }
        if (PlayerPrefs.HasKey(selectedThemeKey))
            selectedTheme = PlayerPrefs.GetInt(selectedThemeKey);
        else
        {
            selectedTheme = 0;
            PlayerPrefs.SetInt(selectedThemeKey, 0);
        }
        if(PlayerPrefs.HasKey(unlockedCarsKey))
            unlockedCars = (uint)PlayerPrefs.GetInt(unlockedCarsKey);
        else
        {
            unlockedCars = 0U;
            PlayerPrefs.SetInt(unlockedCarsKey, (int)0U);
        }
        if (PlayerPrefs.HasKey(unlockedThemesKey)) 
            unlockedThemes = (uint)PlayerPrefs.GetInt(unlockedThemesKey);
        else
        {
            unlockedThemes = 0U;
            PlayerPrefs.SetInt(unlockedThemesKey, (int)0U);
        }
        if(PlayerPrefs.HasKey(highScoreKey))
            highScore = PlayerPrefs.GetInt(highScoreKey);
        else
        {
            highScore = 0;
            PlayerPrefs.SetInt(highScoreKey, highScore);
        }
        if(PlayerPrefs.HasKey(collisionMapKey))
        {
            userMadeMap = true;
            collisionMap = PlayerPrefs.GetString(collisionMapKey);
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
        firstFlagGot = false;
        this.startFlags = startFlags;
        this.startFuel = startFuel;
        gameStartTime = Time.time;
        fuelRemaining = startFuel;
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
        if(map != -1) UpdateMoney(CalculateLossMoney());
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameLost();
        }
    }
    public void GameWon()
    {
        if (state != StateType.GAMEPLAYING) return;
        state = StateType.GAMEWIN;
        if(map != -1) UpdateMoney(CalculateWinMoney());
        if(score > highScore && map != -1)
        {
            highScore = score;
            PlayerPrefs.SetInt(highScoreKey, highScore);
        }
        foreach (GameManagerListener gml in listeners)
        {
            gml.OnGameWon();
        }
    }

    public int CalculateWinMoney()
    {
        
        return map != -1 ? (startFlags - flagsRemaining) * flagValue + 
            (GetPercentFuelRemaining() > 0.5 ? fuelBonus : 0) +
            winBonus : 0;
    }

    public int CalculateLossMoney()
    {
        return map != -1 ? (int)((startFlags - flagsRemaining) * flagValue * 0.5) : 0;
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

    private void ThreadLoop()
    {
        
    }
}

public interface GameManagerListener
{
    public void OnGameLost();
    public void OnGameStarted();
    public void OnGameWon();
    public void OnScoreChanged();
}
