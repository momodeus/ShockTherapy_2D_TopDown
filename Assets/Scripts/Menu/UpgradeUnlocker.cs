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
    [HideInInspector]
    public bool unlocked = false;
    // Start is called before the first frame update
    void Start()
    {
        if((unlocked = GameManager.Instance.IsUpgradeUnlocked((int)upgrade)))
        {
            Unlock();
        }
    }
    public void Preview()
    {
        if (unlocked) return;
        GetComponent<Image>().sprite = lockedSprite;
        lockImage.SetActive(false);
    }

    public void UnPreview()
    {
        if (unlocked) return;
        GetComponent<Image>().sprite = lockedSprite;
        lockImage.SetActive(true);
    }
    public void Lock()
    {
        unlocked = false;
        GetComponent<Image>().sprite = lockedSprite;
        lockImage.SetActive(true);
    }
    public void Unlock()
    {
        unlocked = true;
        GetComponent<Image>().sprite = unlockedSprite;
        lockImage.SetActive(false);
    }
}
