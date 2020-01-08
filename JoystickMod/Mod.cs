using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using UnityEngine;

namespace JoystickMod
{
    public class Mod : ModEntryPoint
    {

        public static GameObject mod;

        public override void OnLoad()
        {
            mod = new GameObject("Joystick Mod");
            //mod.AddComponent<PrintPressedKey>();
            mod.AddComponent<ModDataManager>();
            mod.AddComponent<BlockController>();
            mod.AddComponent<WindowsController>();

            mod.AddComponent<JoystickManagerWindow>();
            mod.AddComponent<JoystickConsoleWindow>();
            mod.AddComponent<JoyAxisMapperWindow>();
            mod.AddComponent<JoyAxisBlockMapperWindow>();


            LanguageManager.Instance.transform.SetParent(mod.transform);
            //mod.AddComponent<MapperWindow>();
            //mod.AddComponent<JoystickBlockController>();
            //JoystickBlockController.Instance.transform.SetParent(mod.transform);
            UnityEngine.Object.DontDestroyOnLoad(mod);
        }

    }
}
