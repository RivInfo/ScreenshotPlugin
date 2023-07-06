using System;
using System.IO;
using UnityEngine;

namespace ScreeshotPlugin
{
    public class ScreenshotScene : MonoBehaviour
    {
        public const string SaveFolderName = "Screenshots";

        [SerializeField] private KeyCode _saveScreenshotKey;

        private static ScreenshotScene _instance;

        private static int _screenCount = 0;

        public KeyCode SaveScreenshotKey
        {
            get => _saveScreenshotKey;
            set => _saveScreenshotKey = value;
        }

        public static ScreenshotScene Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Screenshot Scene Component")
                        .AddComponent<ScreenshotScene>();
                }

                return _instance;
            }
        }

        private static string GetSaveDirectory =>
            Path.Combine(Directory.GetCurrentDirectory(), SaveFolderName);

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
            if (Input.GetKeyDown(_saveScreenshotKey))
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

        private static string CreateScreenName()
        {
            DateTime nowTime = DateTime.Now;
            return "Screen" + string.Format("d4", _screenCount)
                + "-" + nowTime.Hour + "x" + nowTime.Minute + "x" + nowTime.Second
                + "x" + nowTime.Day + "x" + nowTime.Month + "x" + nowTime.Year + ".jpg";
        }

        private static byte[] TextureToJPGByte(Texture texture)
        {
            Texture2D screen = (Texture2D)texture;
            return screen.EncodeToJPG();
        }
    }
}
