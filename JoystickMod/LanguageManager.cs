using System;
using Localisation;
using System.Collections.Generic;
using System.Linq;
using Modding.Common;
using Modding;
using UnityEngine;

namespace JoystickMod
{ 
    public class LanguageManager : SingleInstance<LanguageManager>
    {
        public override string Name { get; } = "Language Manager";

        public Action<ILanguage> OnLanguageChanged;

        private string currentLanguageName;
        private string lastLanguageName = "English";

        public ILanguage CurrentLanguage { get; private set; } = new English();
        Dictionary<string, ILanguage> Dic_Language = new Dictionary<string, ILanguage>
    {
            { "简体中文",new Chinese()},
            { "English",new English()},
            { "日本語",new Japanese()},
    };

        void Awake()
        {
            //OnLanguageChanged += ChangLanguage;
        }

        void Update()
        {
            currentLanguageName = LocalisationManager.Instance.currLangName;

            if (!lastLanguageName.Equals(currentLanguageName))
            {
                lastLanguageName = currentLanguageName;
                ChangLanguage(currentLanguageName);
            }
        }

        void ChangLanguage(string value)
        {
            try
            {
                CurrentLanguage = Dic_Language[value];
            }
            catch
            {
                CurrentLanguage = Dic_Language["English"];
            }
            OnLanguageChanged.Invoke(CurrentLanguage);
        }      
    }

    public interface ILanguage
    {
        //Windows
        string ManagerWindow { get; }
        string ConsoleWindow { get; }
        string AxisWindow { get; }
        string BlockWindow { get; }

        //Manager Window
        string ConsoleWindowToggle { get; }
        string AxisWindowToggle { get; }
        string JoystickSwitchToggle { get; }
        string AxesList { get; }
        string DeleteTipText { get; }
        //Console Window
        string JoystickList { get; }
        string AxisValues { get; }
        string Axis { get; }
        string Value { get; }

        //Axis Window
        string OutputValue{ get; }
        string AxisName { get; }
        string Save { get; }
        string SaveAs { get; }
        string Joystick { get; }
        string Invert { get; }
        string Sensitivity{ get; }
        string Curvature{ get; }
        string Deadzone{ get; }
        string Lerp{ get; }
        string OffsetX{ get; }
        string OffsetY{ get; }

        //Block Window
        string BlockAxisEnabled { get; }
        string BlockAxisInvert { get; }
        string  CurveMax{ get; }
        string  CurveMin{ get; }
        string Block_Axes { get; }
}

    public class Chinese : ILanguage
    {
        //Windows
        public string ManagerWindow { get; } = "摇杆管理窗口 Ctrl+F10";
        public string ConsoleWindow { get; } = "摇杆输出窗口";
        public string AxisWindow { get; } = "轴编辑器窗口";
        public string BlockWindow { get; } = "零件的轴编辑器";

        //Manager Window
        public string ConsoleWindowToggle { get; } = "摇杆输出窗口";
        public string AxisWindowToggle { get; } = "轴编辑器窗口";
        public string JoystickSwitchToggle { get; } = "摇杆总开关";
        public string AxesList { get; } = "轴列表";
        public string DeleteTipText { get; } = "再次点击删除此轴";
        //Console Window
        public string JoystickList { get; } = "摇杆列表";
        public string AxisValues { get; } = "轴输出值列表";
        public string Axis { get; } = "轴";
        public string Value { get; } = "值";

        //Axis Window
        public string OutputValue { get; } = "输出值";
        public string AxisName { get; } = "轴名称";
        public string Save { get; } = "保存";
        public string SaveAs { get; } = "另存为";
        public string Joystick { get; } = "摇杆";
        public string Invert { get; } = "反方向";
        public string Sensitivity { get; } = "灵敏度";
        public string Curvature { get; } = "弯曲度";
        public string Deadzone { get; } = "盲区";
        public string Lerp { get; } = "插值";
        public string OffsetX { get; } = "偏移 X";
        public string OffsetY { get; } = "偏移 Y";

        //Block Window
        public string BlockAxisEnabled { get; } = "轴的使能开关";
        public string BlockAxisInvert { get; } = "轴的反方向";
        public string CurveMax { get; } = "轴曲线最大值";
        public string CurveMin { get; } = "轴曲线最小值";
        public string Block_Axes { get; } = "零件的轴列表";
    }
    public class English : ILanguage
    {
        //Windows
        public string ManagerWindow { get; } = "Joystick Manager Window Ctrl+F10";
        public string ConsoleWindow { get; } = "Joystick Console Window";
        public string AxisWindow { get; } = "Axis Mapper Window";
        public string BlockWindow { get; } = "Axis Block Mapper";

        //Manager Window
        public string ConsoleWindowToggle { get; } = "Console Window";
        public string AxisWindowToggle { get; } = "Axis Mapper Window";
        public string JoystickSwitchToggle { get; } = "Joystick Switch";
        public string AxesList { get; } = "Axes list";
        public string DeleteTipText { get; } = "Click again to delete axis";

        //Console Window
        public string JoystickList { get; } = "Joystick List";
        public string AxisValues { get; } = "Axes Values";
        public string Axis { get; } = "Axis";
        public string Value { get; } = "Value";

        //Axis Window
        public string OutputValue { get; } = "Output Value";
        public string AxisName { get; } = "Axis Name";
        public string Save { get; } = "Save";
        public string SaveAs { get; } = "Save as";
        public string Joystick { get; } = "Joystick";
        public string Invert { get; } = "Invert";
        public string Sensitivity { get; } = "Sensitivity";
        public string Curvature { get; } = "Curvature";
        public string Deadzone { get; } = "Deadzone";
        public string Lerp { get; } = "Lerp";
        public string OffsetX { get; } = "Offset X";
        public string OffsetY { get; } = "Offset Y";

        //Block Window
        public string BlockAxisEnabled { get; } = "Block Axis Enabled";
        public string BlockAxisInvert { get; } = "Block Axis Invert";
        public string CurveMax { get; } = "Curve Max";
        public string CurveMin { get; } = "Curve Min";
        public string Block_Axes { get; } = "Block's Axes";
    }
    public class Japanese : ILanguage
    {
        //Windows
        public string ManagerWindow { get; } = "ジョイスティックmod Ctrl+F10";
        public string ConsoleWindow { get; } = "外部機器制御";
        public string AxisWindow { get; } = "入力設定";
        public string BlockWindow { get; } = "個別ブロック設定";

        //Manager Window
        public string ConsoleWindowToggle { get; } = "外部機器制御ウインドウを開く";
        public string AxisWindowToggle { get; } = "入力設定ウインドウを開く";
        public string JoystickSwitchToggle { get; } = "外部機器を有効化する";
        public string AxesList { get; } = "設定一覧";
        public string DeleteTipText { get; } = "削除するにはもう一度xを押す";

        //Console Window
        public string JoystickList { get; } = "外部機器一覧";
        public string AxisValues { get; } = "入力の強さ";
        public string Axis { get; } = "入力";
        public string Value { get; } = "数値";

        //Axis Window
        public string OutputValue { get; } = "出力の数値";
        public string AxisName { get; } = "入力設定の名前";
        public string Save { get; } = "保存";
        public string SaveAs { get; } = "新規保存";
        public string Joystick { get; } = "外部機器";
        public string Invert { get; } = "反転";
        public string Sensitivity { get; } = "感度";
        public string Curvature { get; } = "なめらかさ";
        public string Deadzone { get; } = "無反応範囲";
        public string Lerp { get; } = "補完";
        public string OffsetX { get; } = "基準点の位置 X";
        public string OffsetY { get; } = "基準点の位置 Y";

        //Block Window
        public string BlockAxisEnabled { get; } = "入力設定を反映する";
        public string BlockAxisInvert { get; } = "反転";
        public string CurveMax { get; } = "動作の最大値";
        public string CurveMin { get; } = "動作の最小値";
        public string Block_Axes { get; } = "参照する設定";
    }
}