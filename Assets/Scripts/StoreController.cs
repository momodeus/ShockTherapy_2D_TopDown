using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StoreController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text moneyText2;
    public GameObject[] themes = new GameObject[MapData.NUM_THEMES];
    public int[] themesCost = new int[MapData.NUM_THEMES];
    void Start()
    {
        for (int i = 0; i < themes.Length; i++)
        {
            (themes[i].transform.GetChild(1).GetComponentInChildren(typeof(Text)) as Text).text = "$" + themesCost[i];
        }
        RefreshUnlocks();
    }
    public void AddMoney()
    {
        GameManager.Instance.UpdateMoney(100);
        moneyText2.text = "$" + GameManager.Instance.Money;
    }
    public void RefreshUnlocks()
    {
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
        (themes[GameManager.Instance.SelectedTheme].GetComponent(typeof(Image)) as Image).color = Color.green;

        moneyText2.text = "$" + GameManager.Instance.Money;

    }
    public void TryBuyTheme(int i)
    {
        if (i < 0 || i > MapData.NUM_THEMES) return;
        if (!GameManager.Instance.UpdateMoney(-themesCost[i + 1])) return;
        GameManager.Instance.UnlockTheme(i);
        moneyText2.text = "$" + GameManager.Instance.Money;
        RefreshUnlocks();
    }

    public void SetActiveTheme(int i)
    {
        if (GameManager.Instance.IsThemeUnlocked(i - 1))
        {
            GameManager.Instance.SelectedTheme = i;
            RefreshUnlocks();
        } else
            print("not unlocked: " + i + "!");
    }
}
