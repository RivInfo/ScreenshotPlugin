using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ScreeshotPlugin
{
    public class ScreenWindowEditor : EditorWindow
    {
        public const string SaveFolderName = "Screenshots";

        private Camera _camera;
        private Texture _texture;

        private bool _newScreenshot = false;
        private string _saveScreenshotsName = "";

        private string GetSaveDirectory =>
            Path.Combine(Directory.GetCurrentDirectory(), SaveFolderName);

        private string GetSaveScreenPath => Path.Combine(GetSaveDirectory, _saveScreenshotsName);

        [MenuItem("Window/ScreenShot")]
        static void Init()
        {
            ScreenWindowEditor window = (ScreenWindowEditor)GetWindow(typeof(ScreenWindowEditor));
            window.Show();
        }

        private void OnEnable()
        {
            FindCamera();
        }

        private void OnGUI()
        {
            GUILayout.Label("Создание скриншота", EditorStyles.boldLabel);
            _camera = (Camera)EditorGUILayout.ObjectField(_camera, typeof(Camera), true);

            EditorGUILayout.Space();

            if (GUILayout.Button("Сделать скриин"))
            {
                TakeScreenshot();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Найти камеру", EditorStyles.miniButtonMid))
            {
                FindCamera();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Открыть в проводнике"))
            {
                if (Directory.Exists(GetSaveDirectory))
                    Process.Start(GetSaveDirectory);
                else
                    Debug.LogWarning($"Directory: {GetSaveDirectory} is not found");
            }

            EditorGUILayout.Space();

            ScreenshotScene[] screenshotScenes = FindObjectsOfType<ScreenshotScene>();

            if (screenshotScenes.Length > 0)
            {
                if (GUILayout.Button($"Удалить все компоненты {nameof(ScreenshotScene)}"))
                {
                    for (int i = 0; i < screenshotScenes.Length; i++)
                    {
                        ScreenshotScene currentScreenshot = screenshotScenes[i];
                        if (currentScreenshot.GetComponents<Component>().Length == 2 &&
                            currentScreenshot.transform.childCount == 0)
                        {
                            DestroyImmediate(currentScreenshot.gameObject);
                        }
                        else
                        {
                            DestroyImmediate(currentScreenshot);
                        }
                    }
                }
            }
            else
            {
                if (GUILayout.Button($"Добавить компонент {nameof(ScreenshotScene)}"))
                {
                    ScreenshotScene screenshotScene =
                        new GameObject("Screenshot Scene Component")
                        .AddComponent<ScreenshotScene>();
                    screenshotScene.SaveScreenshotKey = KeyCode.Alpha1;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Открыть скриншот"))
            {
                string fotoFileName;
                if (Directory.Exists(GetSaveDirectory))
                    fotoFileName = OpenImageDialog(GetSaveDirectory);
                else
                    fotoFileName = OpenImageDialog(Directory.GetCurrentDirectory());
                if (!string.IsNullOrEmpty(fotoFileName))
                {
                    LoadTexture(fotoFileName);
                }
            }

            EditorGUILayout.Space();

            if (File.Exists(GetSaveScreenPath))
            {
                if (_texture)
                {
                    if (GUILayout.Button("Перевести скриншот в чёрно/белый формат"))
                    {
                        ImageToGrayScale();
                    }
                }

                if (_newScreenshot)
                {
                    LoadTexture(GetSaveScreenPath);
                    _newScreenshot = false;
                }

                if (_texture)
                {
                    GUILayout.Box(_texture, GUILayout.Width(position.width - 10),
                        GUILayout.MaxHeight(position.height / 2));
                }
            }
        }

        private string OpenImageDialog(string path)
        {
            return EditorUtility.OpenFilePanelWithFilters("Выберите фотографию",
                        path, new string[] { "Image files", "png,jpg,jpeg" });
        }

        private void LoadTexture(string path)
        {
            Texture2D _texture2d = new Texture2D(1, 1);
            _texture2d.LoadImage(File.ReadAllBytes(path));
            _texture = _texture2d;
        }

        private void ImageToGrayScale()
        {
            Texture2D _texture2d = (Texture2D)_texture;

            Color[] colors = _texture2d.GetPixels();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale);
            }

            _texture2d.SetPixels(colors);
            _texture2d.Apply();
            _texture = _texture2d;
        }

        private void TakeScreenshot()
        {
            if (!Directory.Exists(GetSaveDirectory))
                Directory.CreateDirectory(GetSaveDirectory);

            DateTime nowTime = DateTime.Now;
            string screenName = "Screen" + (_camera == null ? "" : _camera.pixelWidth.ToString())
                + "x" + (_camera == null ? "" : _camera.pixelHeight.ToString())
                + "-" + nowTime.Hour + "x" + nowTime.Minute + "x" + nowTime.Second
                + "x" + nowTime.Day + "x" + nowTime.Month + "x" + nowTime.Year + ".jpg";

            //EditorWindow.FocusWindowIfItsOpen<GameView>();
            EditorApplication.ExecuteMenuItem("Window/General/Game");

            string path = Path.Combine(GetSaveDirectory, screenName);
            ScreenCapture.CaptureScreenshot(path);
            _saveScreenshotsName = screenName;
            Debug.Log(path);

            _newScreenshot = true;
        }

        private void FindCamera()
        {
            _camera = FindObjectOfType<Camera>();
        }
    }
}
