using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GarageController : MonoBehaviour
{

    public BarDisplay[] bars = new BarDisplay[5];
    public UpgradeUnlocker[] upgrades = new UpgradeUnlocker[4];
    public TransitionSceneLoader tsl;

    public Button buyUpgradeButton;
    public Text upgradePriceText;
    public Button buyCarButton;
    public Text carNameText;
    public Text carPriceText;
    public Text moneyText;
    public Image carSelectorImage;
    public Image carImage;
    int[] upgradePrices = new int[]{ 270, 550, 995, 550 };
    int[] carPrices = new int[] { 0, 300, 400, 700, 1000, 1200, 1500, 2000, 5000};
    string[] upgradeNames = new string[]{ "Limit Strap", "Spring Kit", "Race Rack", "Sway Bar" };
    string[] carNames = new string[] { "Arctic Cat Wildcat", "Can Am x3 Maverick", "Can Am x3 RS Turbo", "Honda Talon 100R", "Kawasaki Teryx", "Polaris RZR", "Polaris XP Pro", "Textron Wildcat XX", "Yamaha XYZ" };
    public string[] urls = new string[4];
    public Sprite[] cars = new Sprite[9];
    private int previewedUpgrade = -1;
    private int previewedCar = 0;
    // Start is called before the first frame update
    void Start()
    {
        carImage.sprite = cars[GameManager.Instance.GetSelectedCar()];
        moneyText.text = "$" + GameManager.Instance.GetMoney();
        PreviewUpgrade();
        SetPreviewedCar(GameManager.Instance.GetSelectedCar());
    }
    void SetPreviewedCar(int car)
    {
        if (car < 0 || car > 8) return;
        previewedUpgrade = -1;
        foreach (UpgradeUnlocker uu in upgrades)
        {
            uu.UnPreview();
        }
        PreviewUpgrade();
        previewedCar = car;
        carSelectorImage.sprite = cars[previewedCar];
        carNameText.text = carNames[previewedCar];
        if (GameManager.Instance.IsCarUnlocked(previewedCar))
        {
            carSelectorImage.color = Color.white;
            carPriceText.text = "";
            buyCarButton.gameObject.SetActive(!(GameManager.Instance.GetSelectedCar() == previewedCar));
            buyCarButton.GetComponentInChildren<Text>().text = "Select";
        }
        else
        {
            carSelectorImage.color = Color.black;
            carPriceText.text = "$" + carPrices[previewedCar];
            buyCarButton.gameObject.SetActive(true);
            buyCarButton.interactable = GameManager.Instance.GetMoney() >= carPrices[previewedCar];
            buyCarButton.GetComponentInChildren<Text>().text = "Buy";
        }
        ShowBars();
    }
    public void SwitchCar(bool next)
    {
        SetPreviewedCar(previewedCar + (next ? 1 : -1));
    }

    public void PressedCarButton()
    {
        if(GameManager.Instance.IsCarUnlocked(previewedCar))
        {
            GameManager.Instance.SetSelectedCar(previewedCar);
            carImage.sprite = cars[GameManager.Instance.GetSelectedCar()];
            SetPreviewedCar(previewedCar);
        } else
        {
            if(GameManager.Instance.UpdateMoney(-carPrices[previewedCar]))
            {
                GameManager.Instance.UnlockCar(previewedCar);
                SetPreviewedCar(previewedCar);
                moneyText.text = "$" + GameManager.Instance.GetMoney();
            }
        }
    }
    public void PressedUpgrade(int type)
    {
        SetPreviewedCar(GameManager.Instance.GetSelectedCar());
        foreach(UpgradeUnlocker ul in upgrades)
        {
            ul.UnPreview();
        }
        upgrades[type].Preview();
        if (!upgrades[type].unlocked) previewedUpgrade = type;
        else previewedUpgrade = -1;
        PreviewUpgrade();
    }

    void PreviewUpgrade()
    {
        if (previewedUpgrade == -1) 
        {
            upgradePriceText.text = "";
            buyUpgradeButton.gameObject.SetActive(false);
        } else
        {
            buyUpgradeButton.gameObject.SetActive(true);
            buyUpgradeButton.interactable = GameManager.Instance.GetMoney() >= upgradePrices[previewedUpgrade];
            upgradePriceText.text = "$" + upgradePrices[previewedUpgrade];
            buyUpgradeButton.GetComponentInChildren<Text>().text = "Buy " + upgradeNames[previewedUpgrade];
        }
        ShowBars();
    }

    void ShowBars()
    {
        int j = 0;
        int[] previewedCarValues = GameManager.Instance.GetStatValues(previewedCar);
        foreach (int i in GameManager.Instance.GetStatValues())
        {
            bars[j].SetValues(i, previewedCarValues[j] + (previewedUpgrade == -1 ? 0 : GameManager.UPGRADE_VALUES[previewedUpgrade, j]));
            j++;
        }
    }

    public void ViewProduct()
    {
        if (previewedUpgrade == -1) return;
        Application.OpenURL(urls[previewedUpgrade]);
    }
    public void BuyUpgrade()
    {
        if(GameManager.Instance.UpdateMoney(-upgradePrices[previewedUpgrade]))
        {
            GameManager.Instance.UnlockUpgrade(previewedUpgrade);
            upgrades[previewedUpgrade].Unlock();
            previewedUpgrade = -1;
            moneyText.text = "$" + GameManager.Instance.GetMoney();
            PreviewUpgrade();
        }
    }
    public void Back()
    {
        print("back");
        tsl.LoadScene(0);
    }
}
