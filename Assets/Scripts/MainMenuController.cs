using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject optionsCanvas;
    public Toggle controlSchemeToggle;
    public CustomSwitcher controlSchemeToggleSwitcher;
    public void PlayGame()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void Options()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void Back()
    {
        optionsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Start()
    {
        controlSchemeToggle.isOn = GameManager.Instance.IsSwipeControls();
        controlSchemeToggleSwitcher.OnToggle();
        optionsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void SetControlScheme()
    {
        GameManager.Instance.SetControlScheme(controlSchemeToggle.isOn);
    }
}
