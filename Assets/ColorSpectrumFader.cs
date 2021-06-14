using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSpectrumFader : MonoBehaviour
{
    public float speed = 1.0f;
    private float hue = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().color = Color.HSVToRGB(hue, 1, 1);
        hue += Time.deltaTime * speed;
        hue %= 1.0f;
    }
}
