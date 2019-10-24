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
        public JoyAxis joyAxis = JoyAxis.Default;

        public BlockBehaviour BB;

        private void Awake()
        {
            BB = GetComponent<BlockBehaviour>();
            Events.OnMachineSave += OnSave;
        }

        public void OnSave(PlayerMachineInfo playerMachineInfo)
        {
            foreach (var v in playerMachineInfo.Blocks)
            {
                if (v.Guid == BB.Guid)
                {
                    v.Data.Write("axis-JoyIndex", joyAxis.JoyIndex);
                    v.Data.Write("axis-AxisIndex", joyAxis.AxisIndex);
                    v.Data.Write("axis-Sensitivity", joyAxis.Sensitivity);
                    v.Data.Write("axis-Curvature", joyAxis.Curvature);
                    v.Data.Write("axis-Deadzone", joyAxis.Deadzone);
                    v.Data.Write("axis-Invert", joyAxis.Invert);                
                    v.Data.Write("axis-OffsetX", joyAxis.OffsetX);
                    v.Data.Write("axis-OffsetY", joyAxis.OffsetY);
                    v.Data.Write("axis-Min", joyAxis.Min);
                    v.Data.Write("axis-Max", joyAxis.Max);
                    //v.Data.Write("axis-Lerp", joyAxis.Lerp);
                    break;
                }
            }
            return;
        }

        private void OnDestroy()
        {
            Events.OnMachineSave -= OnSave;
        }
    }
}
