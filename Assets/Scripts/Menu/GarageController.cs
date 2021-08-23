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
    int[] upgradePrices = new int[]{ 135, 275, 500, 550 };
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
        carImage.sprite = cars[GameManager.Instance.SelectedCar];
        moneyText.text = "$" + GameManager.Instance.Money;
        PreviewUpgrade();
        SetPreviewedCar(GameManager.Instance.SelectedCar);
    }

    /// <summary>
    /// updates UI to reflect which car is previewed. 
    /// </summary>
    /// <param name="car"></param>
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
            buyCarButton.gameObject.SetActive(!(GameManager.Instance.SelectedCar == previewedCar));
            buyCarButton.GetComponentInChildren<Text>().text = "Select";
        }
        else
        {
            carSelectorImage.color = Color.black;
            carPriceText.text = "$" + carPrices[previewedCar];
            buyCarButton.gameObject.SetActive(true);
            buyCarButton.interactable = GameManager.Instance.Money >= carPrices[previewedCar];
            buyCarButton.GetComponentInChildren<Text>().text = "Buy";
        }
        ShowBars();
    }


    public void SwitchCar(bool next)
    {
        SetPreviewedCar(previewedCar + (next ? 1 : -1));
    }

    /// <summary>
    /// function called by the "buy/select" button underneath car scroller. 
    /// This will behave differently depending on the current car previewed.
    /// </summary>
    public void PressedCarButton()
    {
        if(GameManager.Instance.IsCarUnlocked(previewedCar))
        {
            GameManager.Instance.SelectedCar = previewedCar;
            carImage.sprite = cars[GameManager.Instance.SelectedCar];
            SetPreviewedCar(previewedCar);
        } else
        {
            if(GameManager.Instance.UpdateMoney(-carPrices[previewedCar]))
            {
                GameManager.Instance.UnlockCar(previewedCar);
                SetPreviewedCar(previewedCar);
                moneyText.text = "$" + GameManager.Instance.Money;
            }
        }
    }

    /// <summary>
    /// This updates UI for previewed upgrades when one is pressed. 
    /// It will un-preview all other upgrades, and set only the selected 
    /// upgrade as previewed. 
    /// </summary>
    /// <param name="type"></param>
    public void PressedUpgrade(int type)
    {
        SetPreviewedCar(GameManager.Instance.SelectedCar);
        foreach(UpgradeUnlocker ul in upgrades)
        {
            ul.UnPreview();
        }
        upgrades[type].Preview();
        if (!upgrades[type].unlocked) previewedUpgrade = type;
        else previewedUpgrade = -1;
        PreviewUpgrade();
    }

    /// <summary>
    /// updates UI to reflect previewed upgrade
    /// </summary>
    void PreviewUpgrade()
    {
        if (previewedUpgrade == -1) 
        {
            upgradePriceText.text = "";
            buyUpgradeButton.gameObject.SetActive(false);
            
        } else
        {
            buyUpgradeButton.gameObject.SetActive(true);
            buyUpgradeButton.interactable = GameManager.Instance.Money >= upgradePrices[previewedUpgrade];
            upgradePriceText.text = "$" + upgradePrices[previewedUpgrade];
            buyUpgradeButton.GetComponentInChildren<Text>().text = "Buy " + upgradeNames[previewedUpgrade];
        }
        ShowBars();
    }

    /// <summary>
    /// Updates stat bars with currenlty previewed car and upgrades
    /// </summary>
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

    /// <summary>
    /// function for button to open web browser with link to ST's website
    /// </summary>
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
            moneyText.text = "$" + GameManager.Instance.Money;
            PreviewUpgrade();
        }
    }
    public void Back()
    {
        tsl.LoadScene(0);
    }
}
