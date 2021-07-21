using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StoreController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text moneyText1;
    public Text moneyText2;
    public GameObject[] cars = new GameObject[9];
    public int[] carsCost = new int[9];
    public GameObject[] themes = new GameObject[MapData.NUM_THEMES];
    public int[] themesCost = new int[MapData.NUM_THEMES];
    void Start()
    {
        for(int i = 0; i < cars.Length; i++)
        {
            (cars[i].transform.GetChild(1).GetComponentInChildren(typeof(Text)) as Text).text = "$" + carsCost[i];
        }
        for (int i = 0; i < themes.Length; i++)
        {
            (themes[i].transform.GetChild(1).GetComponentInChildren(typeof(Text)) as Text).text = "$" + themesCost[i];
        }
        RefreshUnlocks();
    }
    public void AddMoney()
    {
        GameManager.Instance.UpdateMoney(100);
        moneyText1.text = "$" + GameManager.Instance.GetMoney();
        moneyText2.text = "$" + GameManager.Instance.GetMoney();

    }
    public void RefreshUnlocks()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (GameManager.Instance.IsCarUnlocked(i - 1))
            {
                cars[i].transform.GetChild(1).gameObject.SetActive(false);
                cars[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                cars[i].transform.GetChild(1).gameObject.SetActive(true);
                cars[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            (cars[i].GetComponent(typeof(Image)) as Image).color = new Color(0, 0, 0, 0);
        }
        for (int i = 0; i < themes.Length; i++)
        {
            if (GameManager.Instance.IsThemeUnlocked(i - 1))
            {
                themes[i].transform.GetChild(1).gameObject.SetActive(false);
                themes[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                themes[i].transform.GetChild(1).gameObject.SetActive(true);
                themes[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            (themes[i].GetComponent(typeof(Image)) as Image).color = new Color(0, 0, 0, 0);
        }
        (cars[GameManager.Instance.GetSelectedCar()].GetComponent(typeof(Image)) as Image).color = new Color(0, 1, 0, 1);
        (themes[GameManager.Instance.GetSelectedTheme()].GetComponent(typeof(Image)) as Image).color = new Color(0, 1, 0, 1);

        moneyText1.text = "$" + GameManager.Instance.GetMoney();
        moneyText2.text = "$" + GameManager.Instance.GetMoney();

    }
    public void TryBuyCar(int i)
    {
        if (i < 0 || i > 8) return;
        if (!GameManager.Instance.UpdateMoney(-carsCost[i+1])) return;
        GameManager.Instance.UnlockCar(i);

        moneyText1.text = "$" + GameManager.Instance.GetMoney();
        moneyText2.text = "$" + GameManager.Instance.GetMoney();

        RefreshUnlocks();
    }
    public void TryBuyTheme(int i)
    {
        if (i < 0 || i > MapData.NUM_THEMES) return;
        if (!GameManager.Instance.UpdateMoney(-themesCost[i + 1])) return;
        GameManager.Instance.UnlockTheme(i);
        moneyText1.text = "$" + GameManager.Instance.GetMoney();
        moneyText2.text = "$" + GameManager.Instance.GetMoney();
        RefreshUnlocks();
        print("unlocked: " + i);
    }
    public void SetActiveCar(int i)
    {
        if (GameManager.Instance.IsCarUnlocked(i - 1))
        {
            GameManager.Instance.SetSelectedCar(i);
            RefreshUnlocks();
        }
    }
    public void SetActiveTheme(int i)
    {
        if (GameManager.Instance.IsThemeUnlocked(i - 1))
        {
            GameManager.Instance.SetSelectedTheme(i);
            RefreshUnlocks();
        } else
            print("not unlocked: " + i + "!");
    }

    public void LockCars()
    {
        GameManager.Instance.LockAllCars();
        GameManager.Instance.SetSelectedCar(0);
        RefreshUnlocks();
    }
}
