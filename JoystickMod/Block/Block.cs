using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using System.Collections;
using Modding.Blocks;
using System.Collections.ObjectModel;

namespace JoystickMod
{
    public class Block : MonoBehaviour
    {
        public string[] AxesGuid = new string[] { };
        public JoyAxis[] joyAxes = new JoyAxis[] { };
        public bool JoyEnabled = false;
        public float CurveMax = 1f;
        public float CurveMin = -1f;

        public BlockBehaviour BB;
        public Rigidbody rigidbody;

        private bool isFirstFrame = true;

        private void Awake()
        {
            BB = GetComponent<BlockBehaviour>();
            rigidbody = GetComponent<Rigidbody>();
            Events.OnMachineSave += OnSave;

            SafeAwake();
        }
        private void Update()
        {
            if (BB.isSimulating)
            {
                if (isFirstFrame)
                {
                    isFirstFrame = false;
                    joyAxes = BB.BuildingBlock.GetComponent<Block>().joyAxes;
                    BB = GetComponent<BlockBehaviour>();
                    rigidbody = GetComponent<Rigidbody>();
                    if (JoyEnabled) { OnSimulateStart_Enable(); }
                }

                if (JoyEnabled)
                {
                    SimulateUpdateAlways_Enable();
                }
            }
            else
            {
                isFirstFrame = true;
            }
        }
        private void FixedUpdate()
        {
            if (BB.isSimulating && !isFirstFrame)
            {
                if (JoyEnabled)
                {
                    SimulateFixedUpdate_Enable();
                }
            }
        }
        private void LateUpdate()
        {
            if (BB.isSimulating && !isFirstFrame)
            {
                if (JoyEnabled)
                {
                    SimulateLateUpdate_Enable();
                }
            }
        }
        private void OnDestroy()
        {
            Events.OnMachineSave -= OnSave;
        }

        public float GetAxesValue()
        {
            var f = 0f;

            foreach (var a in joyAxes)
            {
                f += a.CurveValue;
            }

            return f;
        }
        public float GetAxesValueDirection()
        {
            var d = 0;

            foreach (var a in joyAxes)
            {
                d += a.DirectionValue;
            }

            if (-0.01f > d)
            {
                return -1f;
            }
            else if (d > 0.01f)
            {
                return 1f;
            }
            else
            {
                return 0f;
            }
        }

        public void OnSave(PlayerMachineInfo playerMachineInfo)
        {
            foreach (var v in playerMachineInfo.Blocks)
            {
                if (v.Guid == BB.Guid)
                {
                    //v.Data.Write("axis-JoyIndex", joyAxis.JoyIndex);
                    //v.Data.Write("axis-AxisIndex", joyAxis.AxisIndex);
                    //v.Data.Write("axis-Sensitivity", joyAxis.Sensitivity);
                    //v.Data.Write("axis-Curvature", joyAxis.Curvature);
                    //v.Data.Write("axis-Deadzone", joyAxis.Deadzone);
                    //v.Data.Write("axis-Invert", joyAxis.Invert);                
                    //v.Data.Write("axis-OffsetX", joyAxis.OffsetX);
                    //v.Data.Write("axis-OffsetY", joyAxis.OffsetY);
                    //v.Data.Write("axis-Min", joyAxis.Min);
                    //v.Data.Write("axis-Max", joyAxis.Max);
                    //v.Data.Write("axis-Lerp", joyAxis.Lerp);
                    //v.Data.Write("axis-Enable", joyAxis.Enable);
                    int index = 0;

                    foreach (var axis in joyAxes)
                    {
                        v.Data.Write("JoyAxis " + index++, axis.Guid);
                    }
                    v.Data.Write("JoyAxis-Number", index);
                    break;
                }
            }
            return;
        }

        public virtual void SafeAwake() { }
        public virtual void OnSimulateStart_Enable() { }
        public virtual void SimulateUpdateAlways_Enable() { }
        public virtual void SimulateFixedUpdate_Enable() { }
        public virtual void SimulateLateUpdate_Enable() { }
    }
}
