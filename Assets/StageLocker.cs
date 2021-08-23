using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLocker : MonoBehaviour
{
    public int stageIndex;
    public Color color;
    // Start is called before the first frame update
    void Awake()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (GameManager.Instance.MaxUnlockedLevel < stageIndex)
        {
            GetComponent<Button>().interactable = false;
            GetComponent<Image>().color = Color.black;
            GetComponentInChildren<Text>().color = Color.black;
            GetComponentInChildren<Text>().text = "???";
        }
        else
        {
            GetComponent<Button>().interactable = true;
            GetComponent<Image>().color = Color.white;
            GetComponentInChildren<Text>().color = color;
            GetComponentInChildren<Text>().text = "STAGE " + (stageIndex + 1);
        }
    }
}
