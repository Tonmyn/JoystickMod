using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoystickMod
{
    public class Joystick
    {
        public string Name { get; set; } = "";
        public int Index { get; set; } = 0;

        public Joystick(string name,int index)
        {
            Name = name;
            Index = index;
        }

        public override string ToString()
        {
            return string.Format("Joystick name:{0},index:{1} ", Name, Index);
        }
    }
}
