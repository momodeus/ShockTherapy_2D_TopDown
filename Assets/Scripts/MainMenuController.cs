using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject optionsCanvas;
    public GameObject carsCanvas;
    public GameObject themesCanvas;
    public GameObject stageSelectCanvas;

    public Button playCustomMapButton;
    public Text highScoreText;
    public Toggle controlSchemeToggle;
    public void PlayGame(int mapIdx)
    {
        GameManager.Instance.SetMap(mapIdx);
        SceneManager.LoadScene("InGameScene");
    }

    public void DesignLevel()
    {
        SceneManager.LoadScene("levelDesign");
    }

    public void LevelSelect()
    {
        mainCanvas.SetActive(false);
        stageSelectCanvas.SetActive(true);
    }
    public void Options()
    {
        mainCanvas.SetActive(false);
        (mainCanvas.GetComponentsInChildren(typeof(Text))[0] as Text).text = "$" + GameManager.Instance.GetMoney();
        optionsCanvas.SetActive(true);
    }

    public void Store()
    {
        mainCanvas.SetActive(false);
        carsCanvas.SetActive(true);
        themesCanvas.SetActive(false);
    }

    public void CarStore()
    {
        themesCanvas.SetActive(false);
        carsCanvas.SetActive(true);
    }

    public void ThemeStore()
    {
        carsCanvas.SetActive(false);
        themesCanvas.SetActive(true);
    }
    public void Back()
    {
        optionsCanvas.SetActive(false);
        carsCanvas.SetActive(false);
        stageSelectCanvas.SetActive(false);
        themesCanvas.SetActive(false);
        (mainCanvas.GetComponentsInChildren(typeof(Text))[0] as Text).text = "$" + GameManager.Instance.GetMoney();
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
        playCustomMapButton.enabled = GameManager.Instance.UserMadeMap();
        (mainCanvas.GetComponentsInChildren(typeof(Text))[0] as Text).text = "$" + GameManager.Instance.GetMoney();
        Back();
    }
    public void SetControlScheme()
    {
        GameManager.Instance.SetControlScheme(controlSchemeToggle.isOn);
    }
}
