using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarDisplay : MonoBehaviour
{
    public Sprite[] lightSprites = new Sprite[12];
    public Image topImage;
    public Image bottomImage;
    [SerializeField]
    [Range(0,11)]
    private int value = 0;
    private int lastValue = 0;
    [SerializeField]
    [Range(0, 11)]
    private int previewValue = 0;
    private int lastPreviewValue = 0;
    public void Update()
    {
        if(value != lastValue)
        {
            SetValues(value, previewValue);
            lastValue = value;
        }
        if(previewValue != lastPreviewValue)
        {
            SetValues(value, previewValue);
            lastPreviewValue = previewValue;
        }
    }
    public int GetValue() => value;
    
    public void SetValues(int newValue, int newPreviewValue)
    {
        if (newValue < 0 || newValue > lightSprites.Length - 1 || 
            newPreviewValue < 0 || newPreviewValue > lightSprites.Length - 1) return;
        
        value = newValue;
        lastValue = value;
        previewValue = newPreviewValue;
        lastPreviewValue = previewValue;
        //2 cases: when value > previewValue, want to show top as previewValue fully lit, and bottom as value negatively lit. 
        //when value < previewValue, want to show top as value fully lit, and bottom as previewValue dimly lit. 
        topImage.sprite = lightSprites[value > previewValue ? previewValue : value];
        bottomImage.sprite = lightSprites[value > previewValue ? value : previewValue];
        bottomImage.color = value > previewValue ? new Color(0, 0, 0, 1f) : new Color(1, 1, 1, 0.5f);
        
    }
}
