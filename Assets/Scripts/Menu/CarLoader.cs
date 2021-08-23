using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarLoader : MonoBehaviour
{
    public Sprite[] cars = new Sprite[9];
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = cars[GameManager.Instance.SelectedCar];
    }
}
