using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StoreController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text moneyText;
    public GameObject[] children = new GameObject[9];
    public int[] cost = new int[9];
    void Start()
    {
        for(int i = 0; i < children.Length; i++)
        {
            (children[i].transform.GetChild(1).GetComponentInChildren(typeof(Text)) as Text).text = "$" + cost[i];
        }
        RefreshUnlocks();
    }
    public void AddMoney()
    {
        GameManager.Instance.UpdateMoney(100);
    }
    public void RefreshUnlocks()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (GameManager.Instance.IsCarUnlocked(i - 1))
            {
                children[i].transform.GetChild(1).gameObject.SetActive(false);
                children[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                children[i].transform.GetChild(1).gameObject.SetActive(true);
                children[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            (children[i].GetComponent(typeof(Image)) as Image).color = new Color(0, 0, 0, 0);

        }
        (children[GameManager.Instance.GetSelectedCar()].GetComponent(typeof(Image)) as Image).color = new Color(0, 1, 0, 1);
    }
    public void TryBuyCar(int i)
    {
        if (i < 0 || i > 8) return;
        if (!GameManager.Instance.UpdateMoney(-cost[i+1])) return;
        GameManager.Instance.UnlockCar(i);

        moneyText.text = "$" + GameManager.Instance.GetMoney();
        RefreshUnlocks();
    }

    public void SetActiveCar(int i)
    {
        if (i < 0 || i > 9) return;

        if (GameManager.Instance.IsCarUnlocked(i - 1))
        {
            GameManager.Instance.SetSelectedCar(i);
            RefreshUnlocks();
        }
    }

    public void LockCars()
    {
        GameManager.Instance.LockAllCars();
        GameManager.Instance.SetSelectedCar(0);
        RefreshUnlocks();
    }
}
