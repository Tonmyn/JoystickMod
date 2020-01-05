using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;

namespace JoystickMod
{
    public abstract class SafeUIBehaviour : MonoBehaviour
    {
        public Rect windowRect = new Rect(0, 0, 192, 128);
        public int windowID { get; protected set; } = ModUtility.GetWindowId();
        public string windowName { get; set; } = "";
        public ILanguage Language;
        GameObject background;

        public static GUIStyle windowStyle = new GUIStyle()
        {
            normal = { background = ModResource.GetTexture("window-background"), textColor = Color.white },
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            border = new RectOffset(4, 4,30, 4),
            padding = new RectOffset(12, 12, 40, 12),
            contentOffset = new Vector2(0, -33)          // -33 = ((30-16)*0.5)-30-(30-40)
        };

        private void Awake()
        {
            this.background = new GameObject("UIBackGround");
            background.transform.SetParent(gameObject.transform);
            background.layer = 13;
            background.AddComponent<BoxCollider>();

            Language = LanguageManager.Instance.CurrentLanguage;
            LanguageManager.Instance.OnLanguageChanged += (value) => { Language = value; };
            SafeAwake();
        }

        void OnGUI()
        {
            if (GameObject.Find("HUD Cam") == null) return;

            if (ShouldShowGUI)
            {
                if (!background.activeSelf)
                    background.SetActive(true);
                Camera hudCamera = GameObject.Find("HUD Cam").GetComponent<Camera>();
                Vector3 leftTop = hudCamera.ScreenPointToRay(new Vector3(windowRect.xMin, hudCamera.pixelHeight - windowRect.yMin, 0)).origin;
                Vector3 rightBottom = hudCamera.ScreenPointToRay(new Vector3(windowRect.xMax, hudCamera.pixelHeight - windowRect.yMax, 0)).origin;

                Vector3 pos = (leftTop + rightBottom) * 0.5f; pos.z += 0.3f;
                background.transform.position = pos;
                Vector3 sca = rightBottom - leftTop; sca.z = 0.1f;
                sca.x = Mathf.Abs(sca.x); sca.y = Mathf.Abs(sca.y);
                background.transform.localScale = sca;



                this.windowRect = GUILayout.Window(this.windowID, this.windowRect, new GUI.WindowFunction(this.WindowContent), this.windowName,windowStyle);
            }
            else
            {
                if (background.activeSelf)
                    background.SetActive(false);
            }
        }
        protected abstract void WindowContent(int windowID);
        public abstract bool ShouldShowGUI { get; set; }

        public virtual void SafeAwake() { }
                                                                                                                       
        public bool AddToggle(string title, bool value = false)
        {
            var rect = windowRect;

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
            {
                GUILayout.Label(title, new GUILayoutOption[] { GUILayout.MaxWidth(rect.width*0.8f), GUILayout.ExpandWidth(false), GUILayout.Height(25f) });
                GUILayout.Space(rect.width * 0.15f);
                value = GUILayout.Toggle(value, "", new GUILayoutOption[] { GUILayout.Width(rect.width * 0.05f), GUILayout.ExpandWidth(false) });
            }
            GUILayout.EndHorizontal();
            return value;
        }
        public float AddSlider(string title, float value, float min = 0f, float max = 1f)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
            {
                GUILayout.Label(title, new GUILayoutOption[] { GUILayout.MaxWidth(windowRect.width*0.35f), GUILayout.ExpandWidth(false), GUILayout.Height(25f) });

                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                value = GUILayout.HorizontalSlider(value, min, max, new GUILayoutOption[] { GUILayout.MaxWidth(windowRect.width * 0.525f), GUILayout.Height(25f) });
                GUILayout.EndVertical();

                GUILayout.Space(10);
                value = float.Parse(GUILayout.TextField(value.ToString("#0.000"), new GUILayoutOption[] { GUILayout.MaxWidth(windowRect.width * 0.125f), GUILayout.ExpandWidth(false), GUILayout.Height(25f) }));
            }
            GUILayout.EndHorizontal();
            return value;
        }
        public int AddMenu(string title, string[] items, int value = 0)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
            {
                GUILayout.Label(title, new GUILayoutOption[] { GUILayout.MaxWidth(75), GUILayout.MinWidth(75) });
                if (GUILayout.Button("<", new GUILayoutOption[] { GUILayout.MaxWidth(30), GUILayout.MinWidth(30) }))
                {
                    if (--value < 0)
                    {
                        value = items.Length - 1;
                    }
                }
                GUILayout.Box(items[value]);
                if (GUILayout.Button(">", new GUILayoutOption[] { GUILayout.MaxWidth(30), GUILayout.MinWidth(30) }))
                {
                    if (++value > items.Length - 1)
                    {
                        value = 0;
                    }
                }
            }
            GUILayout.EndHorizontal();
            return value;
        }
    }
}
