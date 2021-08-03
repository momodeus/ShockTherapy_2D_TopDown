using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject optionsCanvas;
    public GameObject themesCanvas;
    public GameObject stageSelectCanvas;
    public Text mainCanvasMoneyText;
    public TransitionSceneLoader sceneLoader;
    public Button playCustomMapButton;
    public Text highScoreText;
    public Toggle controlSchemeToggle;
    public Color color;
    public void PlayGame(int levelIdx)
    {
        if (levelIdx == -1)
        {
            GameManager.Instance.SetLevelIndex(3);
            GameManager.Instance.SetMap(-1);
        }
        else
        {
            GameManager.Instance.SetLevelIndex(levelIdx);
        }
        
        sceneLoader.LoadScene("InGameScene");
    }

    public void DesignLevel()
    {
        sceneLoader.LoadScene("levelDesign");
    }
    public void EnterGarage()
    {
        sceneLoader.LoadScene("Garage");
    }
    public void EnterQR()
    {
        sceneLoader.LoadScene("QRScanning");
    }
    public void LevelSelect()
    {
        mainCanvas.SetActive(false);
        stageSelectCanvas.SetActive(true);
    }
    public void Options()
    {
        mainCanvas.SetActive(false);
        mainCanvasMoneyText.text = "$" + GameManager.Instance.GetMoney();
        optionsCanvas.SetActive(true);
    }

    public void Store()
    {
        mainCanvas.SetActive(false);
        themesCanvas.SetActive(true);
    }

    public void Back()
    {
        optionsCanvas.SetActive(false);
        stageSelectCanvas.SetActive(false);
        themesCanvas.SetActive(false);
        mainCanvasMoneyText.text = "$" + GameManager.Instance.GetMoney();
        highScoreText.text = "High Score:\n" + GameManager.Instance.GetHighScore();
        mainCanvas.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Start()
    {
        controlSchemeToggle.isOn = GameManager.Instance.IsSwipeControls();
        playCustomMapButton.interactable = GameManager.Instance.UserMadeMap();
        playCustomMapButton.GetComponentInChildren<Text>().color = GameManager.Instance.UserMadeMap() ? color : Color.black;
        mainCanvasMoneyText.text = "$" + GameManager.Instance.GetMoney();
        Back();
    }
    public void SetControlScheme()
    {
        GameManager.Instance.SetControlScheme(controlSchemeToggle.isOn);
    }
}
