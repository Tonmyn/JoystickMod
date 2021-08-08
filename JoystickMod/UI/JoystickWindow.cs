using JoystickMod;
using Modding;
using Modding.Blocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Block = JoystickMod.Block;

public class WindowsController : MonoBehaviour
{
    public static event Action<bool> OnToggleMapper;
    public static event Action<JoyAxis> OnChangedJoyAxis;
    public static event Action<JoystickMod.Block> OnChangedBlock;
    private bool opened = false;
    private BlockBehaviour lastBlock = new BlockBehaviour();
    public static JoyAxis currentJoyAxis = JoyAxis.Default;
    private JoyAxis lastJoyAxis = currentJoyAxis;
 
    private void Update()
    {
        if (BlockMapper.IsOpen)
        {
            var block = BlockMapper.CurrentInstance.Block;
            if (BlockController.isAxisBlock(block) )
            {
                if (block != lastBlock)
                {
                    //var axis = BlockController.GetJoyAxisData(block);
                    OnChangedBlock?.Invoke(/*axis*/block.GetComponent<JoystickMod.Block>());
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

    public static GUIStyle titleStyle2 = new GUIStyle()
    {
        fontSize = 12,
        normal = new GUIStyleState() { textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f) }
    };
    public static GUIStyle titleStyle3 = new GUIStyle()
    {
        fontSize = 12,
        normal = new GUIStyleState() { textColor = Color.yellow}
    };
}

public class JoystickConsoleWindow : SafeUIBehaviour
{
    public override bool ShouldShowGUI { get; set; } = false;

    private List<Joystick> joysticks = new List<Joystick>();
    private List<string> joystickNamesList = new List<string>();
    private List<string> AxisValues = new List<string>();
    private int selectedIndex = 0;
    private int joyIndex = 0;

    public override void SafeAwake()
    {
        windowRect = new Rect(15f, 100f, 356f, 180f);
        Mod.mod.GetComponent<JoystickManagerWindow>().OnJoysticksChanged += refreshJoystick;
        refreshJoystick(Mod.mod.GetComponent<JoystickManagerWindow>().Joysticks);
    }

    public void refreshJoystick(List<Joystick> joysticks)
    {
        joystickNamesList.Clear();
        this.joysticks = joysticks;
        joysticks.ForEach((joy) => { joystickNamesList.Add(joy.Name); /*Debug.Log(joy);*/ });
    }

    protected override void WindowContent(int windowID)
    {
        windowName = Language.ConsoleWindow;

        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(Language.JoystickList);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            selectedIndex = GUILayout.SelectionGrid(selectedIndex, /*Input.GetJoystickNames()*/joystickNamesList.ToArray(), 1,new GUILayoutOption[] { GUILayout.MaxWidth(450)});
            joyIndex = joysticks[selectedIndex].Index;
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(Language.AxesList);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            AxisValues.Clear();
            for (int i = 0; i < 10; i++)
            {
                AxisValues.Add(string.Format("{0} {1} - {2}: {3}",Language.Axis, i,Language.Value, JoyAxis.GetAxisValue(joyIndex, i).ToString("# 0.00")));
            }
            GUILayout.SelectionGrid(-1, AxisValues.ToArray(), 2, new GUILayoutOption[] { GUILayout.MinWidth(100) });
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}

class JoystickManagerWindow : SafeUIBehaviour
{
    public override bool ShouldShowGUI { get; set; } = false;
    public List<Joystick> Joysticks = new List<Joystick>();

    public event Action<List<Joystick>> OnJoysticksChanged;

    public JoystickConsoleWindow consoleWindow;
    public JoyAxisMapperWindow AxisMapperWindow;
    public JoyAxisBlockMapperWindow AxisBlockMapperWindow;

    private ModDataManager dataManager;
    private JoyAxis lastJoyAxis;
    private string[] lastjoystickNames;
   
    public override void SafeAwake()
    {
        dataManager = Mod.mod.GetComponent<ModDataManager>();

        windowRect = new Rect(50, 50, 300, dataManager.ModData.joyAxes.Length * 30f + 150f);

        refreshJoysticks();
    }


    void Update()
    {
        if (consoleWindow == null || AxisMapperWindow == null || AxisBlockMapperWindow == null)
        {
            consoleWindow = Mod.mod.GetComponent<JoystickConsoleWindow>();
            AxisMapperWindow = Mod.mod.GetComponent<JoyAxisMapperWindow>();
            AxisBlockMapperWindow = Mod.mod.GetComponent<JoyAxisBlockMapperWindow>();
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F10))
        {
            if (WindowsController.IsBuilding() && !StatMaster.inMenu)
            {
                ShouldShowGUI = !ShouldShowGUI;
            }

            if (ShouldShowGUI == false)
            {
                consoleWindow.ShouldShowGUI = false;
            }

            //Joysticks.ForEach(joy => Debug.Log(joy.ToString()));
        }

        //if (!lastjoystickNames.Equals(Input.GetJoystickNames()))
        //{
        //    refreshJoysticks();
        //}
    }

    void refreshJoysticks()
    {
        var ignorStr = new List<string>() { "ABCDEFGHIJKLMNOPQRSTUVWXYZ" };

        lastjoystickNames =  Input.GetJoystickNames();
        Joysticks.Clear();
        int index = 0;
        //lastjoystickNames.ToList().ForEach((str) => Joysticks.Add(new Joystick(Regex.Replace(Regex.Replace(str, "  ", ""), "[\n]", " "), index++)));
        foreach (var str in lastjoystickNames)
        {
            var exist = false;
            ignorStr.ForEach(match => exist = str.Contains(match));
            if (!exist)
            {
                Joysticks.Add(new Joystick(Regex.Replace(Regex.Replace(str, "  ", ""), "[\n]", " "), index++));
            }
        }

        Joysticks.Add(new Joystick("Mouse", 1024));
        OnJoysticksChanged?.Invoke(Joysticks);
    }

    protected override void WindowContent(int windowID)
    {
        windowName = Language.ManagerWindow;
        var value =Joysticks.Count > 0;

        GUILayout.BeginVertical();
        {
            consoleWindow.ShouldShowGUI = AddToggle(Language.ConsoleWindowToggle, consoleWindow.ShouldShowGUI && value);
            AxisMapperWindow.ShouldShowGUI = AddToggle(Language.AxisWindowToggle, AxisMapperWindow.ShouldShowGUI && value);
            Block.JoystickEnabled = AddToggle(Language.JoystickSwitchToggle, Block.JoystickEnabled);
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(Language.AxesList + ": ", WindowsController.titleStyle2);
            if (lastJoyAxis != null) GUILayout.Label(Language.DeleteTipText, WindowsController.titleStyle3);
        }
        GUILayout.EndHorizontal();

        try
        {
            foreach (var joyAxis in dataManager.ModData.joyAxes)
            {
                AddJoyaxisButton(joyAxis);
            }
        }
        catch(Exception e)
        { Debug.Log(e.Message); }


        GUI.DragWindow(new Rect(0, 0, windowRect.width, 24f));
    }

    public void AddJoyaxisButton(JoyAxis joyAxis)
    {
        var rect = windowRect;

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(joyAxis.Name, new GUILayoutOption[] { GUILayout.MaxWidth(rect.width * 0.7f) }))
            {
                AxisMapperWindow.ShouldShowGUI = true;
                AxisMapperWindow.JoyAxis = joyAxis.Copy();
            }

            if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.MaxWidth(rect.width * 0.15f) }))
            {
                if (lastJoyAxis == null)
                {
                    lastJoyAxis = joyAxis.Copy();
                    StartCoroutine(delaySeconds(5f, () => lastJoyAxis = null));
                }
                else
                {
                    if (lastJoyAxis.Guid == joyAxis.Guid)
                    {
                        dataManager.RemoveAxis(joyAxis);
                        lastJoyAxis = null;
                    }
                    else
                    {
                        lastJoyAxis = joyAxis.Copy();
                        StartCoroutine(delaySeconds(5f, () => lastJoyAxis = null));
                    }
                }

               IEnumerator delaySeconds(float seconds,Action action)
                {
                    yield return new WaitForSeconds(seconds);
                    action?.Invoke();
                }
            }

            if (AxisBlockMapperWindow.ShouldShowGUI == true)
            {
                if (GUILayout.Button(" + ", new GUILayoutOption[] { GUILayout.MaxWidth(rect.width * 0.15f) }))
                {
                    var joyAxes = AxisBlockMapperWindow.block.joyAxes.ToList();
                    if (!joyAxes.Exists(match => match.Guid == joyAxis.Guid))
                    {
                        joyAxes.Add(joyAxis);
                        AxisBlockMapperWindow.block.joyAxes = joyAxes.ToArray();
                    }
                }
            }
        }
        GUILayout.EndHorizontal();

    }
}

public class JoyAxisBlockMapperWindow : SafeUIBehaviour
{
    public override bool ShouldShowGUI { get; set; } = false;

    public JoyAxisMapperWindow AxisMapperWindow;

    public Block block;

    private JoyAxis lastJoyAxis;
    public override void SafeAwake()
    {
        windowRect = new Rect(15f, 100f, 300f, 300f);
    

        WindowsController.OnToggleMapper += (value) => { ShouldShowGUI = value; };
        WindowsController.OnChangedBlock += (value) => { block = value; };

        AxisMapperWindow = Mod.mod.GetComponent<JoyAxisMapperWindow>();
    }

    protected override void WindowContent(int windowID)
    {
        windowName = Language.BlockWindow;

        GUILayout.BeginVertical();
        {
            block.AxisEnabled = AddToggle(Language.BlockAxisEnabled, block.AxisEnabled);
            block.Invert = AddToggle(Language.BlockAxisInvert, block.Invert);
            block.CurveMax = AddSlider(Language.CurveMax, block.CurveMax, 0, 3f);
            block.CurveMin = AddSlider(Language.CurveMin, block.CurveMin, -3f, 0);
            block.Lerp = AddSlider(Language.Lerp, block.Lerp,0f,5f);
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(Language.Block_Axes + ":", WindowsController.titleStyle2);
            if (lastJoyAxis != null) GUILayout.Label(Language.DeleteTipText + ":", WindowsController.titleStyle3);
        }
        GUILayout.EndHorizontal();
      

        GUILayout.BeginVertical();
        {
            try
            {
                foreach (var axis in block.joyAxes)
                {
                    AddAxisButton(axis);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0, 0, windowRect.width, 24f));
    }

    public void AddAxisButton(JoyAxis joyAxis)
    {
        var rect = windowRect;

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(joyAxis.Name, new GUILayoutOption[] { GUILayout.MaxWidth(rect.width * 0.7f) }))
            {
                AxisMapperWindow.ShouldShowGUI = true;
                AxisMapperWindow.JoyAxis = joyAxis.Copy();
            }


            if (GUILayout.Button(" x ", new GUILayoutOption[] { GUILayout.MaxWidth(rect.width * 0.3f) }))
            {
                if (lastJoyAxis == null)
                {
                    lastJoyAxis = joyAxis.Copy();
                    StartCoroutine(delaySeconds(5f, () => lastJoyAxis = null));
                }
                else
                {
                    if (lastJoyAxis.Guid == joyAxis.Guid)
                    {
                        var list = block.joyAxes.ToList();
                        list.RemoveAll(match => match.Guid == joyAxis.Guid);
                        block.joyAxes = list.ToArray();
                        lastJoyAxis = null;
                    }
                    else
                    {
                        lastJoyAxis = joyAxis.Copy();
                        StartCoroutine(delaySeconds(5f, () => lastJoyAxis = null));
                    }
                }

                IEnumerator delaySeconds(float seconds, Action action)
                {
                    yield return new WaitForSeconds(seconds);
                    action?.Invoke();
                }
            }

        }
        GUILayout.EndHorizontal();
    }
}

public class JoyAxisMapperWindow : SafeUIBehaviour
{
    public override bool ShouldShowGUI { get; set; } = false;

    private Rect _graphRect;
    private Rect _lastGraphRect;
    private Texture2D _graphTex;
    private Color[] _resetTex;

    public JoyAxis JoyAxis = JoyAxis.Default.Copy();

    private ModDataManager dataManager;
    private List<Joystick> joysticks = new List<Joystick>();
    private List<string> joystickNamesList = new List<string>();
    private int selectedIndex = 0;

    public override void SafeAwake()
    {
        windowRect = new Rect(15f, 100f, 300f, 300f); 

        WindowsController.OnChangedJoyAxis += (value) => { JoyAxis = value.Copy(); };

        dataManager = Mod.mod.GetComponent<ModDataManager>();
        Mod.mod.GetComponent<JoystickManagerWindow>().OnJoysticksChanged += refreshJoystick;
        refreshJoystick(Mod.mod.GetComponent<JoystickManagerWindow>().Joysticks);
    }

    public void refreshJoystick(List<Joystick> joysticks)
    {
        joystickNamesList.Clear();
        this.joysticks = joysticks;
        joysticks.ForEach((joy) => { joystickNamesList.Add(joy.Name);/* Debug.Log(joy); */});
    }

    protected override void WindowContent(int windowID)
    {
        windowName = Language.AxisWindow;

        Rect crossRect = new Rect((windowRect.width - windowRect.width * 0.9f) * 0.5f, 46f * 0f + 23 , windowRect.width * 0.9f, windowRect.width * 0.9f);
        _graphRect = crossRect;

        DrawCrossRect(crossRect, Color.gray);
        DrawGraph();
        GUI.Label(new Rect(30, 50, 150, 24), Language.OutputValue + ": " + JoyAxis.CurveValue.ToString("#0.00"));
        FillRect(new Rect(crossRect.x + crossRect.width * 0.5f + JoyAxis.RawValue * crossRect.width * 0.5f, crossRect.y, 1, crossRect.height), Color.yellow);

        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
        {
            GUILayout.Label(Language.AxisName, new GUILayoutOption[] { GUILayout.MaxWidth(75f) }); 
            JoyAxis.Name = GUILayout.TextField(JoyAxis.Name, new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.MaxWidth(windowRect.width * 0.45f) });
            
            if (GUILayout.Button(Language.Save))
            {
                if (dataManager.ModData.joyAxes.ToList().Exists(match => match.Guid == JoyAxis.Guid))
                {
                    dataManager.ReplaceAxis(JoyAxis.Copy());
                }
                else
                {
                    dataManager.AddAxis(JoyAxis.Copy());
                }
            }

            if (GUILayout.Button(Language.SaveAs))
            {
                JoyAxis.Guid = Guid.NewGuid();
                dataManager.AddAxis(JoyAxis.Copy());
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        {
            //JoyAxis.JoyIndex = AddMenu(Language.Joystick, /*Input.GetJoystickNames()*/joystickNamesList.ToArray(), JoyAxis.JoyIndex);
            selectedIndex = AddMenu(Language.Joystick, /*Input.GetJoystickNames()*/joystickNamesList.ToArray(), selectedIndex);
            JoyAxis.JoyIndex = joysticks[selectedIndex].Index;
            JoyAxis.AxisIndex = AddMenu(Language.Axis, new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" }, JoyAxis.AxisIndex);

            JoyAxis.Invert = AddToggle(Language.Invert, JoyAxis.Invert);

            JoyAxis.Sensitivity = AddSlider(Language.Sensitivity, JoyAxis.Sensitivity, 0f, 5f);
            JoyAxis.Curvature = AddSlider(Language.Curvature, JoyAxis.Curvature, 0f, 3f);
            JoyAxis.Deadzone = AddSlider(Language.Deadzone, JoyAxis.Deadzone, 0f, 0.5f);
            JoyAxis.Lerp = AddSlider(Language.Lerp, JoyAxis.Lerp, 0f, 1f);
            JoyAxis.OffsetX = AddSlider(Language.OffsetX, JoyAxis.OffsetX, -1f, 1f);
            JoyAxis.OffsetY = AddSlider(Language.OffsetY, JoyAxis.OffsetY, -1f, 1f);
        }
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
    }

    private static readonly Texture2D RectTexture = new Texture2D(1, 1);
    private static readonly GUIStyle RectStyle = new GUIStyle();
    private static readonly Color CurrentColor = new Color();

    private void DrawCrossRect(Rect rect, Color color)
    {
        DrawRect(new Rect(rect.x, rect.y, rect.width * 0.5f + 1, rect.height * 0.5f), color);
        DrawRect(new Rect(rect.x + rect.width * 0.5f - 1, rect.y, rect.width * 0.5f + 1, rect.height * 0.5f), color);
        DrawRect(new Rect(rect.x, rect.y + rect.height * 0.5f - 1, rect.width * 0.5f + 1, rect.height * 0.5f + 1), color);
        DrawRect(new Rect(rect.x + rect.width * 0.5f - 1, rect.y + rect.height * 0.5f - 1, rect.width * 0.5f + 1, rect.height * 0.5f + 1), color);
    }
    private void DrawRect(Rect position, Color color)
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
        if (JoyAxis.Changed || _graphRect != _lastGraphRect)
        {
            _graphTex.SetPixels(_resetTex);
            float step = 0.5f / _graphTex.width;
            for (float xValue = -1; xValue < 1; xValue += step)
            {
                float yValue = JoyAxis.Process(xValue);
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
    private void FillRect(Rect position, Color color)
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
