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
        public string Name = "JoyAxis";

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

        public float Lerp
        {
            get { return _lerp; }
            set {_lerp = value; }
        }
        private float _lerp;

        public bool Enable { get; set; } =false;


        /// <summary>
        /// Is true if the axis tuning has been changed since the last call.
        /// </summary>
        public bool Changed
        {
            get { bool tmp = _changed; _changed = false; return tmp; }
            set { _changed = value; }
        }
        private bool _changed = true;


        public float Min { get; set; } = -1f;
        public float Max { get; set; } = 1f;

        public float RawValue { get { return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", JoyIndex, AxisIndex)) * 10f; } }
        public float CurveValue { get { return Process(RawValue); } } 

        //public float Value { get { return getAxisValue(this) * Sign; } }
        //public float Value_Sign { get { return getAxisValue(this) * Sign; } }
        public int Value_Direction { get { return (int)ConverJoyValueToFloat()/* * (Invert == true ? -1 : 1)*/; } }

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

        public JoyAxis(bool enable, int joyIndex, int axisIndex, float sensitivity, float curvature, float deadzone, bool invert, float offsetX, float offsetY, float min, float max,float lerp)
        {
            Enable = enable;

            JoyIndex = joyIndex;
            AxisIndex = axisIndex;

            Min = min;
            Max = max;

            Sensitivity = sensitivity;
            Curvature = curvature;
            Deadzone = deadzone;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Invert = invert;
            Lerp = lerp;
        }

        public JoyAxis(JoyAxis joyAxis)
        {
            Enable = joyAxis.Enable;

            JoyIndex = joyAxis.JoyIndex;
            AxisIndex = joyAxis.AxisIndex;

            Min = joyAxis.Min;
            Max = joyAxis.Max;

            Sensitivity = joyAxis.Sensitivity;
            Curvature = joyAxis.Curvature;
            Deadzone = joyAxis.Deadzone;
            OffsetX = joyAxis.OffsetX;
            OffsetY = joyAxis.OffsetY;
            Invert = joyAxis.Invert;
            Lerp = joyAxis.Lerp;
        }

        public static JoyAxis Default { get { return new JoyAxis(false,0, 0, 1f, 1f, 0f, false, 0f, 0f, -1f, 1f, 1f); } }

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
            return /*Mathf.MoveTowards(CurveValue, */Mathf.Clamp(value + OffsetY, -1, 1)/*, Lerp)*/;
        }

        public static float getAxisValue(JoyAxis axis)
        {
            return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", axis.JoyIndex, axis.AxisIndex)) * 10f;
        }

        //public float getAxisValue()
        //{
        //    //return Input.GetAxis("Joy0Axis" + ((int)axis).ToString()) * 10f;

        //    return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", JoyIndex, AxisIndex)) * 10f;
        //}

        public static float GetAxisValue(int joyIndex, int axisIndex)
        {
            //return Input.GetAxis("Joy0Axis" + ((int)axis).ToString()) * 10f;

            return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", joyIndex, axisIndex)) * 10f;
        }

        public float ConverJoyValueToFloat()
        {
            if (-0.01f > CurveValue)
            {
                return -1f;
            }
            else if (CurveValue > 0.01f)
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
            return string.Format("Enable:{0},JoyIndex:{1},AxisIndex:{2},Sensitivity:{3},Curvature:{4},Deadzone:{5},Invert:{6},OffsetX:{7},OffsetY:{8},Min:{9},Max:{10},Lerp:{11}",Enable, JoyIndex, AxisIndex, Sensitivity, Curvature, Deadzone, Invert, OffsetX, OffsetY, Min, Max, Lerp);
        }

    }
}
