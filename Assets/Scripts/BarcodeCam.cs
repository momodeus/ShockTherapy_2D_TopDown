﻿/*
* Copyright 2012 ZXing.Net authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Threading;

using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class BarcodeCam : MonoBehaviour
{
    // Texture for encoding test
    public RawImage cameraImage;
    private WebCamTexture camTexture;
    private Thread qrThread;
    private Color32[] c;
    private int W, H;
    private bool found = false;
    private bool isQuit;
    public string lastResult;
    public TransitionSceneLoader sceneLoader;
    public GameObject foundPopup;


    private void OnGUI()
    {
        cameraImage.texture = camTexture;
    }
    void OnEnable()
    {
        if (camTexture != null)
        {
            camTexture.Play();
            W = camTexture.width;
            H = camTexture.height;
        }
    }

    void OnDisable()
    {
        if (camTexture != null)
        {
            camTexture.Pause();
        }
    }

    void OnDestroy()
    {
        qrThread.Abort();
        camTexture.Stop();
    }

    // It's better to stop the thread by itself rather than abort it.
    void OnApplicationQuit()
    {
        isQuit = true;
    }

    void Start()
    {
        lastResult = "http://www.google.com";
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height; // 480;
        camTexture.requestedWidth = Screen.width; //640;
        OnEnable();
#if UNITY_ANDROID
        cameraImage.transform.localScale = new Vector3(-1, -1, 1);
#endif
        qrThread = new Thread(DecodeQR);
        qrThread.Start();
        foundPopup.gameObject.SetActive(false);
    }

    void Update()
    {
        if (c == null)
        {
            c = camTexture.GetPixels32();
        }
        if(found && GameManager.VerifyCollisionMap(lastResult))
        {
            foundPopup.gameObject.SetActive(true);
            found = false;
        }
    }

    public void HidePopupWindow()
    {
        foundPopup.gameObject.SetActive(false);
    }
    public void PlayScannedMap()
    {
        print("got: \n" + Compression.DecompressCollisionMap(lastResult));
        GameManager.Instance.scannedMap = Compression.DecompressCollisionMap(lastResult);
        GameManager.Instance.Map = -2;
        GameManager.Instance.LevelIndex = 3;
        sceneLoader.LoadScene("InGameScene");
    }
    void DecodeQR()
    {
        // create a reader with a custom luminance source
        var barcodeReader = new BarcodeReader { AutoRotate = false };//, TryHarder = false };

        while (true)
        {
            if (isQuit)
                break;

            try
            {
                // decode the current frame
                var result = barcodeReader.Decode(c, W, H);
                if (result != null)
                {
                    lastResult = result.Text;
                    found = true;
                }

                // Sleep a little bit and set the signal to get the next frame
                Thread.Sleep(200);
                c = null;
            }
            catch
            {
            }
        }
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}
