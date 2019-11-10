using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoystickMod
{
    public class JoysitckData
    {
        public bool consoloWindowShowEnabled = false;
        public bool joystickManagerWindowShowEnabled = false;

        public List<JoyAxis> joyAxes;

        public void AddJoyAxis(JoyAxis joyAxis)
        {
            joyAxes.Add(joyAxis);
        }

        public void Load()
        {

        }

        public void Save()
        {

        }
    }
}
