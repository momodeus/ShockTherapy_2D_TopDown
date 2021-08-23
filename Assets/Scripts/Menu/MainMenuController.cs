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
    public Button swipeControlButton, touchControlButton;
    public Image swipeControlImage, touchControlImage;
    public Sprite checkedSprite, uncheckedSprite;
    public Color color;
    public void PlayGame(int levelIdx)
    {
        if (levelIdx == -1)
        {
            GameManager.Instance.LevelIndex = 3;
            GameManager.Instance.Map = -1;
        }
        else
        {
            GameManager.Instance.LevelIndex = levelIdx;
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
        mainCanvasMoneyText.text = "$" + GameManager.Instance.Money;
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
        mainCanvasMoneyText.text = "$" + GameManager.Instance.Money;
        highScoreText.text = "High Score:\n" + GameManager.Instance.HighScore;
        mainCanvas.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Start()
    {
        swipeControlImage.sprite = GameManager.Instance.SwipeControls ? checkedSprite : uncheckedSprite;
        touchControlImage.sprite = GameManager.Instance.SwipeControls ? uncheckedSprite : checkedSprite;
        playCustomMapButton.interactable = GameManager.Instance.UserMadeMap;
        playCustomMapButton.GetComponentInChildren<Text>().color = GameManager.Instance.UserMadeMap ? color : Color.black;
        mainCanvasMoneyText.text = "$" + GameManager.Instance.Money;
        Back();
    }
    public void SetControlScheme(bool swipeControls)
    {
        GameManager.Instance.SwipeControls = swipeControls;
        swipeControlImage.sprite = GameManager.Instance.SwipeControls ? checkedSprite : uncheckedSprite;
        touchControlImage.sprite = GameManager.Instance.SwipeControls ? uncheckedSprite : checkedSprite;
    }
}
