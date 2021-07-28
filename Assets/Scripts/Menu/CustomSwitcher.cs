using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomSwitcher : MonoBehaviour
{

    public Text text;
    public string onText;
    public string offText;
    public Toggle toggle;
    public void OnToggle()
    {
        text.text = toggle.isOn ? onText : offText;
    }
}
