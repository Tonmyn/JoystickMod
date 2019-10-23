using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoystickMod
{
    public class JoyAxis
    {

        public int JoyIndex = 0;
        public int AxisIndex = 0;
        //public bool Flipped = true;


        /* TUNING:
          * Following properties flip the changed flag on edit.
          * Changed flag is used for redrawing the graph.
          */
        public float Sensitivity
        {
            get { return _sensitivity; }
            set { _changed |= value != _sensitivity; _sensitivity = value; }
        }
        private float _sensitivity;
       
        public float Curvature
        {
            get { return _curvature; }
            set { _changed |= value != _curvature; _curvature = value; }
        }
        private float _curvature;

        public float Deadzone
        {
            get { return _deadzone; }
            set { _changed |= value != _deadzone; _deadzone = value; }
        }
        private float _deadzone;

        public float OffsetX
        {
            get { return _offx; }
            set { _changed |= value != _offx; _offx = value; }
        }
        private float _offx;

        public float OffsetY
        {
            get { return _offy; }
            set { _changed |= value != _offy; _offy = value; }
        }
        private float _offy;

        public bool Invert
        {
            get { return _invert; }
            set { _changed |= value != _invert; _invert = value; }
        }
        private bool _invert;

        public bool Smooth { get; set; }

        private bool _changed = true;

        /// <summary>
        /// Is true if the axis tuning has been changed since the last call.
        /// </summary>
        public bool Changed
        {
            get { bool tmp = _changed; _changed = false; return tmp; }
        }

        //public float Offset = 0;
        public float Min { get; set; } = -1f;
        public float Max { get; set; } = 1f;

        public float RawValue { get { return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", JoyIndex, AxisIndex)) * 10f; } }
        public float CurveValue { get { return Process(RawValue); } }

        //public float Value { get { return getAxisValue(this) * Sign; } }
        //public float Value_Sign { get { return getAxisValue(this) * Sign; } }
        //public int Value_Direction { get { return (int)ConverJoyValueToFloat(/*AxisIndex*/) * Sign; } }

        //public int Sign { get { return Flipped ? 1 : -1; } }

        //public JoyAxis(int joyIndex,int axisIndex,float sensitivity,float curvature ,float deadzone,bool flipped,float offsetX,float offsetY,float min,float max)
        //{
        //    JoyIndex = 0;
        //    AxisIndex = 0;
        //    Flipped = true;

        //    //Offset = 0;
        //    Min = -1f;
        //    Max = 1f;

        //    Sensitivity = 1;
        //    Curvature = 1;
        //    Deadzone = 0;
        //    OffsetX = 0;
        //    OffsetY = 0;
        //    Invert = false;
        //    Smooth = false;
        //}

        public JoyAxis(int joyIndex, int axisIndex, float sensitivity, float curvature, float deadzone, bool invert, float offsetX, float offsetY, float min, float max)
        {
            JoyIndex = joyIndex;
            AxisIndex = axisIndex;
            //Flipped = flipped;

            //Offset = 0;
            Min = min;
            Max = max;

            Sensitivity = sensitivity;
            Curvature = curvature;
            Deadzone = deadzone;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Invert = /*false*/invert;
            Smooth = false;
        }

        public static JoyAxis Default { get { return new JoyAxis(0, 0, 1f, 1f, 0f, false, 0f, 0f, -1f, 1f); } }

        /// <summary>
        /// Returns processed output value for given input value.
        /// Intended for drawing graph.
        /// </summary>
        /// <param name="input">Input value in range [-1, +1]</param>
        /// <returns>Output value in range [-1, +1]</returns>
        public float Process(float input)
        {
            input += OffsetX;
            float value;
            if (Mathf.Abs(input) < Deadzone)
                return 0 + OffsetY;
            else
                value = input > 0 ? input - Deadzone : input + Deadzone;
            value *= Sensitivity * (Invert ? -1 : 1);
            value = value > 0 ? Mathf.Pow(value, Curvature) : -Mathf.Pow(-value, Curvature);
            return Mathf.Clamp(value + OffsetY, -1, 1);
        }

        public static float getAxisValue(JoyAxis axis)
        {
            //return Input.GetAxis("Joy0Axis" + ((int)axis).ToString()) * 10f;

            return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", axis.JoyIndex, axis.AxisIndex)) * 10f;
        }

        public float getAxisValue()
        {
            //return Input.GetAxis("Joy0Axis" + ((int)axis).ToString()) * 10f;

            return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", JoyIndex, AxisIndex)) * 10f;
        }

        public static float getAxisValue(int joyIndex, int axisIndex)
        {
            //return Input.GetAxis("Joy0Axis" + ((int)axis).ToString()) * 10f;

            return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", joyIndex, axisIndex)) * 10f;
        }

        public float ConverJoyValueToFloat()
        {
            if (-0.01f > getAxisValue(/*AxisIndex*/))
            {
                return -1f;
            }
            else if (getAxisValue(/*AxisIndex*/) > 0.01f)
            {
                return 1f;
            }
            else
            {
                return 0f;
            }

        }

        public override string ToString()
        {
            return string.Format("JoyIndex:{0},AxisIndex:{1},Flipped:{2},OffsetX:{3},OffsetY:{4},Min:{5},Max:{6}", JoyIndex, AxisIndex, Invert, OffsetX, OffsetY, Min, Max);
        }

    }
}
