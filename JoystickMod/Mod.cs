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

        public GameObject mod;

        public override void OnLoad()
        {
            mod = new GameObject("Joystick Mod");
            //mod.AddComponent<PrintPressedKey>();
            mod.AddComponent<BlockController>();
            mod.AddComponent<WindowsController>();
            mod.AddComponent<JoystickConsoleWindow>();
            mod.AddComponent<JoyAxisMapperWindow>();
           
            //mod.AddComponent<MapperWindow>();
            //mod.AddComponent<JoystickBlockController>();
            //JoystickBlockController.Instance.transform.SetParent(mod.transform);
            UnityEngine.Object.DontDestroyOnLoad(mod);
        }

    }
}
