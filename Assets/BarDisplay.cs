using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarDisplay : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[12];
    [SerializeField][Range(0,11)]
    private int value = 0;
    private int lastValue = 0;
    public void Update()
    {
        if(value != lastValue)
        {
            SetValue(value);
            lastValue = value;
        }
    }
    public int GetValue() => value;
    
    public void SetValue(int newValue)
    {
        if (newValue < 0 || newValue > sprites.Length - 1) return;
        GetComponent<Image>().sprite = sprites[newValue];
        value = newValue;
    }
}
