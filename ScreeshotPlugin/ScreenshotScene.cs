using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ScreeshotPlugin

public class ScreenshotScene : MonoBehaviour
{
    [SerializeField] private KeyCode _savetKey;

    public const string SaveFolderName = "Screenshots";

    private ScreenshotScene _instance;

    private static int _screenCount = 0;

    public KeyCode SaveScreenshotKey
    {
        get { return _savetKey; }
        set { _savetKey = value; }
    }

    public ScreenshotScene Instance 
    { 
        get 
        {
            if (_instance != null)
                return _instance;
            else
            {
                ScreenshotScene screenshot = new GameObject("Screenshot Scene Component")
                    .AddComponent<ScreenshotScene>();
                return screenshot;
            }
        } 
    }

    private static string GetSaveDirectory => Path.Combine(Directory.GetCurrentDirectory(), SaveFolderName);

    private void OnEnable()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    private void OnDisable()
    {
        if (_instance == this)
            _instance = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_savetKey))
        {
            SaveScreenshotInSaveFolder(TakeScreenshot());
        }
    }

    public static Texture TakeScreenshot()
    {       
        _screenCount++;
        return ScreenCapture.CaptureScreenshotAsTexture();
    }

    public static void SaveScreenshotInSaveFolder(Texture screenshot)
    {
        string screenName = CreateScreenName();

        File.WriteAllBytes(Path.Combine(GetSaveDirectory, screenName), 
            TextureToJPGByte(screenshot));

        Debug.Log("Screenshot path: " + Path.Combine(GetSaveDirectory, screenName));
    }

    public static void SaveScreenshotInCache(Texture screenshot)
    {
        string screenName = CreateScreenName();

        File.WriteAllBytes(Path.Combine(Application.temporaryCachePath, screenName),
            TextureToJPGByte(screenshot));

        Debug.Log("Screenshot path: " + Path.Combine(Application.temporaryCachePath, screenName));
    }

    private static byte[] TextureToJPGByte(Texture texture)
    {
        Texture2D screen = (Texture2D)texture;
        return screen.EncodeToJPG();
    }

    private static string CreateScreenName()
    {
        DateTime nowTime = DateTime.Now;
        return "Screen" + string.Format("d4", _screenCount)
            + "-" + nowTime.Hour + "x" + nowTime.Minute + "x" + nowTime.Second
            + "x" + nowTime.Day + "x" + nowTime.Month + "x" + nowTime.Year + ".jpg";
    }
}
