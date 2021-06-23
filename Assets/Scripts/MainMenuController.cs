using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject optionsCanvas;
    public GameObject storeCanvas;
    public Toggle controlSchemeToggle;
    public void PlayGame()
    {
        SceneManager.LoadScene("InGameScene");
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
        (storeCanvas.GetComponentsInChildren(typeof(Text))[0] as Text).text = "$" + GameManager.Instance.GetMoney();
        storeCanvas.SetActive(true);
    }
    public void Back()
    {
        optionsCanvas.SetActive(false);
        storeCanvas.SetActive(false);
        (mainCanvas.GetComponentsInChildren(typeof(Text))[0] as Text).text = "$" + GameManager.Instance.GetMoney();
        mainCanvas.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Start()
    {
        controlSchemeToggle.isOn = GameManager.Instance.IsSwipeControls();
        (mainCanvas.GetComponentsInChildren(typeof(Text))[0] as Text).text = "$" + GameManager.Instance.GetMoney();
        Back();
    }
    public void SetControlScheme()
    {
        GameManager.Instance.SetControlScheme(controlSchemeToggle.isOn);
    }
}
