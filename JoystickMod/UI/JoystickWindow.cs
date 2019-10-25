using JoystickMod;
using Modding;
using Modding.Blocks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowsController : MonoBehaviour
{
    public static event Action<bool> OnToggleMapper;
    public static event Action<JoyAxis> OnChangedBlock;
    private bool opened = false;
    private BlockBehaviour lastBlock = new BlockBehaviour();


    public void Awake()
    {


    }

 

    private void Update()
    {
        if (BlockMapper.IsOpen)
        {
            var block = BlockMapper.CurrentInstance.Block;
            if (BlockController.isAxisBlock(block) )
            {
                if (block != lastBlock)
                {
                    var axis = BlockController.GetJoyAxisData(block);
                    OnChangedBlock?.Invoke(axis);
                    lastBlock = block;
                }
                if (opened == false)
                {
                    OnToggleMapper?.Invoke(true);
                    opened = true;
                }      
            }
            else
            {
                if (opened)
                {
                    OnToggleMapper?.Invoke(false);
                    opened = false;
                }
            }
        }
        else
        {
            if (opened)
            {
                OnToggleMapper?.Invoke(false);
                opened = false;
            }   
        }
    }



    public static bool IsBuilding()
    {
        List<string> scene = new List<string> { "INITIALISER", "TITLE SCREEN", "LevelSelect", "LevelSelect1", "LevelSelect2", "LevelSelect3" };

        if (SceneManager.GetActiveScene().isLoaded)
        {
            if (!scene.Exists(match => match == SceneManager.GetActiveScene().name))
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public static bool AddToggle(string title, bool value = false)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
        {
            GUILayout.Label(title, new GUILayoutOption[] { GUILayout.MinWidth(75), GUILayout.ExpandWidth(false), GUILayout.Height(25f) });
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, "", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.ExpandWidth(false) });
        }
        GUILayout.EndHorizontal();
        return value;
    }
    public static float AddSlider(string title, float value, float min = 0f, float max = 1f)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
        {
            GUILayout.Label(title, new GUILayoutOption[] { GUILayout.MinWidth(75), GUILayout.ExpandWidth(false), GUILayout.Height(25f) });
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            value = GUILayout.HorizontalSlider(value, min, max, new GUILayoutOption[] { GUILayout.MinWidth(75), GUILayout.Height(15f) });
            GUILayout.EndVertical();
            //GUILayout.Space(width * 0.05f);
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            value = float.Parse(GUILayout.TextField(value.ToString("#0.000"), new GUILayoutOption[] { GUILayout.MinWidth(50), GUILayout.ExpandWidth(false), GUILayout.Height(25f) }));
        }
        GUILayout.EndHorizontal();
        return value;
    }
    public static int AddMenu(string title, string[] items, int value = 0)
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


    private static readonly Texture2D RectTexture = new Texture2D(1, 1);
    private static readonly GUIStyle RectStyle = new GUIStyle();
    private static readonly Color CurrentColor = new Color();

    internal static void DrawCrossRect(Rect rect, Color color)
    {
        DrawRect(new Rect(rect.x, rect.y, rect.width * 0.5f + 1, rect.height * 0.5f), color);
        DrawRect(new Rect(rect.x + rect.width * 0.5f - 1, rect.y, rect.width * 0.5f + 1, rect.height * 0.5f), color);
        DrawRect(new Rect(rect.x, rect.y + rect.height * 0.5f - 1, rect.width * 0.5f + 1, rect.height * 0.5f + 1), color);
        DrawRect(new Rect(rect.x + rect.width * 0.5f - 1, rect.y + rect.height * 0.5f - 1, rect.width * 0.5f + 1, rect.height * 0.5f + 1), color);
    }
    internal static void DrawRect(Rect position, Color color)
    {
        FillRect(new Rect(
            position.x,
            position.y,
            position.width,
            1),
            color);

        FillRect(new Rect(
            position.x,
            position.y + position.height - 1,
            position.width,
            1),
            color);

        FillRect(new Rect(
            position.x,
            position.y,
            1,
            position.height),
            color);

        FillRect(new Rect(
            position.x + position.width - 1,
            position.y,
            1,
            position.height),
            color);
    }
    internal static void FillRect(Rect position, Color color)
    {
        if (color != CurrentColor)
        {
            RectTexture.SetPixel(0, 0, color);
            RectTexture.Apply();

            RectStyle.normal.background = RectTexture;
        }
        GUI.Box(position, GUIContent.none, RectStyle);
    }

}

public class JoystickConsoleWindow : SafeUIBehaviour
{
    public override bool ShouldShowGUI { get; set; } = false;

    
    private List<string> AxisValues = new List<string>();
    private int joyIndex = 0;

    public override void SafeAwake()
    {
        windowRect = new Rect(15f, 100f, 356f, 180f);
        windowName = "Joystick Console Window Ctrl+F10";
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F10))
        {
            if (WindowsController.IsBuilding() && !StatMaster.inMenu)
            {
                ShouldShowGUI = !ShouldShowGUI;
            }
        }
    }

    protected override void WindowContent(int windowID)
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Select Joystick");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            joyIndex = GUILayout.SelectionGrid(joyIndex, Input.GetJoystickNames(), 1);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Axis Values");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            AxisValues.Clear();
            for (int i = 0; i < 10; i++)
            {
                AxisValues.Add(string.Format("Axis {0} - Value: {1}", i, JoyAxis.GetAxisValue(joyIndex, i).ToString("# 0.00")));
            }
            GUILayout.SelectionGrid(-1, AxisValues.ToArray(), 2, new GUILayoutOption[] { GUILayout.MinWidth(100) });
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}

public class JoyAxisMapperWindow : SafeUIBehaviour
{
    public override bool ShouldShowGUI { get; set; } = false;

    private Rect _graphRect;
    private Rect _lastGraphRect;
    private Texture2D _graphTex;
    private Color[] _resetTex;

    private JoyAxis _joyAxis = JoyAxis.Default;

    public override void SafeAwake()
    {
        windowRect = new Rect(15f, 100f, 300f, 300f); 
        windowName = "Axis Mapper";

        WindowsController.OnToggleMapper += (value) => { ShouldShowGUI = value; };
        WindowsController.OnChangedBlock += (value) => { _joyAxis = value; };
    }


    protected override void WindowContent(int windowID)
    {
        Rect crossRect = new Rect((windowRect.width - windowRect.width * 0.9f) * 0.5f, 46f * 0f + 23, windowRect.width * 0.9f, windowRect.width * 0.9f);
        _graphRect = crossRect;

        DrawCrossRect(crossRect, Color.gray);
        DrawGraph();
        GUI.Label(new Rect(30, 50, 150, 24), "Output Value: " + _joyAxis.CurveValue.ToString("#0.00"));

        FillRect(new Rect(crossRect.x + crossRect.width * 0.5f + _joyAxis.RawValue * crossRect.width * 0.5f, crossRect.y, 1, crossRect.height), Color.yellow);

        _joyAxis.Enable = WindowsController.AddToggle("Enable", _joyAxis.Enable);
        _joyAxis.JoyIndex  =WindowsController.AddMenu("Joystick", Input.GetJoystickNames(), _joyAxis.JoyIndex);
        _joyAxis.AxisIndex  = WindowsController.AddMenu("Axis", new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, _joyAxis.AxisIndex);

        _joyAxis.Invert = WindowsController.AddToggle("Invert", _joyAxis.Invert);

        _joyAxis.Sensitivity = WindowsController.AddSlider("Sensitivity", _joyAxis.Sensitivity, 0f, 5f);
        _joyAxis.Curvature = WindowsController.AddSlider("Curvature", _joyAxis.Curvature, 0f, 3f);
        _joyAxis.Deadzone = WindowsController.AddSlider("Deadzone", _joyAxis.Deadzone, 0f, 0.5f);
        _joyAxis.Lerp = WindowsController.AddSlider("Lerp", _joyAxis.Lerp, 0f, 1f);

        GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
    }

    private static readonly Texture2D RectTexture = new Texture2D(1, 1);
    private static readonly GUIStyle RectStyle = new GUIStyle();
    private static readonly Color CurrentColor = new Color();

    internal static void DrawCrossRect(Rect rect, Color color)
    {
        DrawRect(new Rect(rect.x, rect.y, rect.width * 0.5f + 1, rect.height * 0.5f), color);
        DrawRect(new Rect(rect.x + rect.width * 0.5f - 1, rect.y, rect.width * 0.5f + 1, rect.height * 0.5f), color);
        DrawRect(new Rect(rect.x, rect.y + rect.height * 0.5f - 1, rect.width * 0.5f + 1, rect.height * 0.5f + 1), color);
        DrawRect(new Rect(rect.x + rect.width * 0.5f - 1, rect.y + rect.height * 0.5f - 1, rect.width * 0.5f + 1, rect.height * 0.5f + 1), color);
    }
    internal static void DrawRect(Rect position, Color color)
    {
        FillRect(new Rect(
            position.x,
            position.y,
            position.width,
            1),
            color);

        FillRect(new Rect(
            position.x,
            position.y + position.height - 1,
            position.width,
            1),
            color);

        FillRect(new Rect(
            position.x,
            position.y,
            1,
            position.height),
            color);

        FillRect(new Rect(
            position.x + position.width - 1,
            position.y,
            1,
            position.height),
            color);
    }

    private void DrawGraph()
    {
        if (_graphTex == null || _graphRect != _lastGraphRect)
        {
            _graphTex = new Texture2D((int)_graphRect.width, (int)_graphRect.height);
            for (int i = 0; i < _graphTex.width; i++)
                for (int j = 0; j < _graphTex.height; j++)
                    _graphTex.SetPixel(i, j, Color.clear);
            _resetTex = _graphTex.GetPixels();
        }
        if (_joyAxis.Changed || _graphRect != _lastGraphRect)
        {
            _graphTex.SetPixels(_resetTex);
            float step = 0.5f / _graphTex.width;
            for (float xValue = -1; xValue < 1; xValue += step)
            {
                float yValue = _joyAxis.Process(xValue);
                if (yValue <= -1f || yValue >= 1f) continue;
                float xPixel = (xValue + 1) * _graphTex.width / 2;
                float yPixel = (yValue + 1) * _graphTex.height / 2;
                _graphTex.SetPixel(Mathf.RoundToInt(xPixel), Mathf.RoundToInt(yPixel), Color.white);
            }
            _graphTex.Apply();
        }
        _lastGraphRect = _graphRect;
        GUILayout.Box(_graphTex);
    }
    internal static void FillRect(Rect position, Color color)
    {
        if (color != CurrentColor)
        {
            RectTexture.SetPixel(0, 0, color);
            RectTexture.Apply();

            RectStyle.normal.background = RectTexture;
        }

        GUI.Box(position, GUIContent.none, RectStyle);
        //GUILayout.Box(GUIContent.none, RectStyle);
    }
}
