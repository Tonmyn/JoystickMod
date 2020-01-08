using Modding;
using Modding.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace JoystickMod
{
    public class JoyAxis:Element
    {
        public static int MouseJoyIndex { get; } = 1024;

        [Modding.Serialization.CanBeEmpty]
        public int JoyIndex;
        [Modding.Serialization.CanBeEmpty]
        public int AxisIndex;
    [Modding.Serialization.CanBeEmpty]
        public string Name;

        /* TUNING:
          * Following properties flip the changed flag on edit.
          * Changed flag is used for redrawing the graph.
          */
        [Modding.Serialization.CanBeEmpty]
        public float Sensitivity { get { return _sensitivity; } set { _changed |= value != _sensitivity; _sensitivity = value; } }
        private float _sensitivity;
        [Modding.Serialization.CanBeEmpty]
        public float Curvature { get { return _curvature; } set { _changed |= value != _curvature; _curvature = value; } }           
        private float _curvature;
        [Modding.Serialization.CanBeEmpty]
        public float Deadzone { get { return _deadzone; } set { _changed |= value != _deadzone; _deadzone = value; } }
        private float _deadzone;
        [Modding.Serialization.CanBeEmpty]
        public float OffsetX { get { return _offx; } set { _changed |= value != _offx; _offx = value; } }
        private float _offx;
        [Modding.Serialization.CanBeEmpty]
        public float OffsetY { get { return _offy; } set { _changed |= value != _offy; _offy = value; } }
        private float _offy;
        [Modding.Serialization.CanBeEmpty]
        public bool Invert { get { return _invert; } set { _changed |= value != _invert; _invert = value; } }
        private bool _invert;
        [Modding.Serialization.CanBeEmpty]
        public float Lerp { get { return _lerp; } set { _lerp = value; } }
        private float _lerp;
        //[Modding.Serialization.CanBeEmpty]
        //public bool Enable { get; set; } =false;
        [Modding.Serialization.CanBeEmpty]
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Is true if the axis tuning has been changed since the last call.
        /// </summary>
        public bool Changed
        { get { bool tmp = _changed; _changed = false; return tmp; } set { _changed = value; } }
        private bool _changed = true;

        [Modding.Serialization.CanBeEmpty]
        public float Min { get; set; } = -1f;
        [Modding.Serialization.CanBeEmpty]
        public float Max { get; set; } = 1f;

        public float RawValue { get { return GetAxisValue(JoyIndex, AxisIndex); } }
        public float CurveValue { get { return Process(RawValue); } } 
        public int DirectionValue { get { return (int)ConverAxisValueToNormal(); } }

        public JoyAxis( int joyIndex, int axisIndex, string name,float sensitivity, float curvature, float deadzone, bool invert, float offsetX, float offsetY, float min, float max,float lerp)
        {
            JoyIndex = joyIndex;
            AxisIndex = axisIndex;
            Name = name;

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
        public JoyAxis()
        {
            JoyIndex = 0;
            AxisIndex = 0;
            Name = "Joy Axis";

            Sensitivity = 1f;
            Curvature = 1f;
            Deadzone = 0f;
            Invert = false;
            OffsetX = OffsetY = 0f;
            Min = -1f;
            Max = 1f;
            Lerp = 1f;
        }
        public static JoyAxis Default { get { return new JoyAxis(0, 0,"Joy Axis", 1f, 1f, 0f, false, 0f, 0f, -1f, 1f, 1f); } }

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

        public JoyAxis Copy()
        {
            JoyAxis joyAxis = new JoyAxis();

            joyAxis.Guid = Guid;

            joyAxis.JoyIndex = JoyIndex;
            joyAxis.AxisIndex = AxisIndex;
            joyAxis.Name = Name;

            joyAxis.Min = Min;
            joyAxis.Max = Max;

            joyAxis.Sensitivity = Sensitivity;
            joyAxis.Curvature = Curvature;
            joyAxis.Deadzone =Deadzone;
            joyAxis.OffsetX = OffsetX;
            joyAxis.OffsetY = OffsetY;
            joyAxis.Invert = Invert;
            joyAxis.Lerp =Lerp;

            return joyAxis;
        }

        public static float GetAxisValue(JoyAxis axis)
        {
            return GetAxisValue( axis.JoyIndex, axis.AxisIndex);
        }
        public static float GetAxisValue(int joyIndex, int axisIndex)
        {
            if (joyIndex == MouseJoyIndex)
            {
                float value = 0;
                
                switch (axisIndex)
                {
                    case 0: value = Input.mousePosition.x; break;
                    case 1: value = Input.mousePosition.y; break;
                    case 2: value = Input.GetAxis("Mouse X"); break;
                    case 3: value = Input.GetAxis("Mouse Y"); break;
                    default: value = 0f; break;
                }
                return value;
            }
            else
            {
                return Input.GetAxisRaw(string.Format("Joy{0}Axis{1}", joyIndex, axisIndex)) * 10f;
            }
        }
        public float ConverAxisValueToNormal()
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
            return string.Format("name:{0},JoyIndex:{1},AxisIndex:{2},Sensitivity:{3},Curvature:{4},Deadzone:{5},Invert:{6},OffsetX:{7},OffsetY:{8},Min:{9},Max:{10},Lerp:{11}", Name, JoyIndex, AxisIndex, Sensitivity, Curvature, Deadzone, Invert, OffsetX, OffsetY, Min, Max, Lerp);
        }
    }
}
