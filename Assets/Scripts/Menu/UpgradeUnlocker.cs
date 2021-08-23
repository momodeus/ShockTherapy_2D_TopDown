using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnlocker : MonoBehaviour
{
    public enum UpgradeType
    {
        LIMIT_STRAP,
        SPRING_KIT,
        RACE_RACK,
        SWAY_BAR
    }

    public UpgradeType upgrade;
    public Sprite lockSprite;
    public GameObject lockImage;
    public Sprite lockedSprite, unlockedSprite;
    public GameObject previewOnCar;
    [HideInInspector]
    public bool unlocked = false;
    private Image img;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        if((unlocked = GameManager.Instance.IsUpgradeUnlocked((int)upgrade)))
        {
            Unlock();
        }
    }
    public void Preview()
    {
        if (unlocked) return;
        img.sprite = lockedSprite;
        previewOnCar.SetActive(true);
        lockImage.SetActive(false);
    }

    public void UnPreview()
    {
        if (unlocked) return;
        img.sprite = lockedSprite;
        previewOnCar.SetActive(false);
        lockImage.SetActive(true);
    }
    public void Lock()
    {
        unlocked = false;
        img.sprite = lockedSprite;
        previewOnCar.SetActive(false);
        lockImage.SetActive(true);
    }
    public void Unlock()
    {
        unlocked = true;
        img.sprite = unlockedSprite;
        previewOnCar.SetActive(false);
        lockImage.SetActive(false);
    }
}
