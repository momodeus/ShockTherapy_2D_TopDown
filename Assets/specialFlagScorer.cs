using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class specialFlagScorer : MonoBehaviour
{
    public GameObject multiplier;
    [HideInInspector]
    private bool iMultiplied = false;
    public bool isMultiplied { get => iMultiplied; set { iMultiplied = value;  multiplier.SetActive(iMultiplied); } }
    // Start is called before the first frame update
    void Start()
    {
        multiplier.SetActive(iMultiplied);
    }
}
