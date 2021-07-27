using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GarageController : MonoBehaviour
{

    public BarDisplay[] bars = new BarDisplay[5];
    public UpgradeUnlocker[] upgrades = new UpgradeUnlocker[4];
    public TransitionSceneLoader tsl;

    public Button button;
    public Text priceText;
    public Text moneyText;
    int[] upgradePrices = new int[]{ 270, 550, 995, 550 };
    string[] upgradeNames = new string[]{ "Limit Strap", "Spring Kit", "Race Rack", "Sway Bar" };
    public string[] urls = new string[4];
    private int previewed = -1;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.LockAllUpgrades();
        GameManager.Instance.UpdateMoney(1000);
        moneyText.text = "$" + GameManager.Instance.GetMoney();
        Preview();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressedUpgrade(int type)
    {
        foreach(UpgradeUnlocker ul in upgrades)
        {
            ul.UnPreview();
        }
        upgrades[type].Preview();
        if (!upgrades[type].unlocked) previewed = type;
        else previewed = -1;
        Preview();
    }

    void Preview()
    {
        if (previewed == -1) 
        {
            priceText.text = "";
            button.gameObject.SetActive(false);
        } else
        {
            button.gameObject.SetActive(true);
            button.interactable = GameManager.Instance.GetMoney() >= upgradePrices[previewed];
            print(upgradePrices[previewed]);
            priceText.text = "$" + upgradePrices[previewed];
            button.GetComponentInChildren<Text>().text = "Buy " + upgradeNames[previewed];
        }
        int j = 0;
        foreach (int i in GameManager.Instance.GetStatValues())
        {
            bars[j].SetValue(i + (previewed == -1 ? 0 : GameManager.UPGRADE_VALUES[previewed, j]));
            j++;
        }
    }


    public void ViewProduct()
    {
        if (previewed == -1) return;
        Application.OpenURL(urls[previewed]);
    }
    public void BuyUpgrade()
    {
        if(GameManager.Instance.UpdateMoney(-upgradePrices[previewed]))
        {
            GameManager.Instance.UnlockUpgrade(previewed);
            upgrades[previewed].Unlock();
            previewed = -1;
            moneyText.text = "$" + GameManager.Instance.GetMoney();
            Preview();
        }
    }
    public void Back()
    {
        print("back");
        tsl.LoadScene(0);
    }
}
