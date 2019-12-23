using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using Modding.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JoystickMod
{
    public class ModData : Element
    {
        [Modding.Serialization.RequireToValidate]
        public bool consoloWindowShowEnabled;
        [Modding.Serialization.RequireToValidate]
        public bool joystickManagerWindowShowEnabled;
        [Modding.Serialization.CanBeEmpty]
        public JoyAxis[] joyAxes;

        public ModData()
        {
            consoloWindowShowEnabled = false;

            joystickManagerWindowShowEnabled = false;

            joyAxes = new JoyAxis[] { /*new JoyAxis() */};
        }

        public override string ToString()
        {
            string str = "";
            str = str + "Consolo Window Show Enabled: " + consoloWindowShowEnabled.ToString() + "\n";
            str = str + " Joystick Manager Window Show Enabled: " + joystickManagerWindowShowEnabled.ToString() + "\n";

            foreach (var ja in joyAxes)
            {
                str = str + ja.ToString() + "\n";
            }

            return str;
        }
    }
}
