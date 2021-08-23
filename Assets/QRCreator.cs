using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;

public class QRCreator : MonoBehaviour
{
    private Texture2D encoded;
    // Start is called before the first frame update
    void Start()
    {
        encoded = new Texture2D(256, 256);
        if(GameManager.Instance.UserMadeMap)
        {
            var color32 = Encode(Compression.CompressCollisionMap(GameManager.Instance.CollisionMap), 
                encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
        } else
        {
            gameObject.SetActive(false);
        }
        GetComponent<Image>().sprite = Sprite.Create(encoded, new Rect(0, 0, encoded.width, encoded.height), new Vector2(0,0));
    }

    private static Color32[] Encode(string textForEncoding, int w, int h)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = h,
                Width = w
            }
        };
        return writer.Write(textForEncoding);
    }

}
