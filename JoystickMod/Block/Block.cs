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
        public bool Invert = false;
        public float CurveMax = 1f;
        public float CurveMin = -1f;
        public float Lerp = 1f;

        public BlockBehaviour BB;
        public Rigidbody rigidbody;

        private bool isFirstFrame = true;

        private void Awake()
        {
            BB = GetComponent<BlockBehaviour>();
            rigidbody = GetComponent<Rigidbody>();
            Events.OnMachineSave += OnSave;
            Mod.mod.GetComponent<ModDataManager>().OnModDataChanged += RefreshAxes;
            SafeAwake();
        }

        private void Update()
        {
            if (BB.isSimulating)
            {
                if (isFirstFrame)
                {
                    isFirstFrame = false;
                    BB = GetComponent<BlockBehaviour>();
                    rigidbody = GetComponent<Rigidbody>();

                    var block = BB.BuildingBlock.GetComponent<Block>();
                    joyAxes = block.joyAxes;
                    JoyEnabled = block.JoyEnabled;
                    Invert = block.Invert;
                    CurveMax = block.CurveMax;
                    CurveMin = block.CurveMin;
                    Lerp = block.Lerp;
                                       
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
            f = Invert ? f * -1f : f;
            
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
        public void RefreshAxes(ModData modData)
        {
            var list = new List<JoyAxis>();

            modData.joyAxes.ToList().ForEach(doSomething);
            this.joyAxes = list.ToArray();

            void doSomething(JoyAxis joyAxis)
            {
                if (this.joyAxes.ToList().Exists(match => match.Guid == joyAxis.Guid))
                {
                    list.Add(joyAxis);
                }
            }
        }

        public void OnSave(PlayerMachineInfo playerMachineInfo)
        {
            foreach (var v in playerMachineInfo.Blocks)
            {
                if (v.Guid == BB.Guid)
                {
                    int index = 0;

                    foreach (var axis in joyAxes)
                    {
                        v.Data.Write("JoyAxis " + index++, axis.Guid.ToString());
                    }
                    v.Data.Write("BlockAxis-Number", index);
                    v.Data.Write("BlockAxis-Enabled", JoyEnabled);
                    v.Data.Write("BlockAxis-Invert", Invert);
                    v.Data.Write("BlockAxis-CurveMax", CurveMax);
                    v.Data.Write("BlockAxis-CurveMin", CurveMin);
                    v.Data.Write("BlockAxis-Lerp", Lerp);
                    break;
                }
            }
            return;
        }
        public void OnLoad(Modding.Blocks.BlockInfo BlockInfo)
        {
            try { JoyEnabled = BlockInfo.Data.ReadBool("BlockAxis-Enabled"); } catch { }
            try { joyAxes = formatDataToJoyAxes(BlockInfo); } catch { }
            try { Invert = BlockInfo.Data.ReadBool("BlockAxis-Invert"); } catch { }
            try { CurveMax = BlockInfo.Data.ReadFloat("BlockAxis-CurveMax"); } catch { }
            try { CurveMin = BlockInfo.Data.ReadFloat("BlockAxis-CurveMin"); } catch { }
            try { Lerp = BlockInfo.Data.ReadFloat("BlockAxis-Lerp"); } catch { }

            JoyAxis formatDataToJoyAxis(Modding.Blocks.BlockInfo blockInfo)
            {
                var axis = JoyAxis.Default;


                if (blockInfo.Data.HasKey("axis-JoyIndex"))
                {
                    axis.JoyIndex = blockInfo.Data.ReadInt("axis-JoyIndex");
                }
                if (blockInfo.Data.HasKey("axis-AxisIndex"))
                {
                    axis.AxisIndex = blockInfo.Data.ReadInt("axis-AxisIndex");
                }
                if (blockInfo.Data.HasKey("axis-Sensitivity"))
                {
                    axis.Sensitivity = blockInfo.Data.ReadFloat("axis-Sensitivity");
                }
                if (blockInfo.Data.HasKey("axis-Curvature"))
                {
                    axis.Curvature = blockInfo.Data.ReadFloat("axis-Curvature");
                }
                if (blockInfo.Data.HasKey("axis-Deadzone"))
                {
                    axis.Deadzone = blockInfo.Data.ReadFloat("axis-Deadzone");
                }
                if (blockInfo.Data.HasKey("axis-Invert"))
                {
                    axis.Invert = blockInfo.Data.ReadBool("axis-Invert");
                }
                if (blockInfo.Data.HasKey("axis-OffsetX"))
                {
                    axis.OffsetX = blockInfo.Data.ReadFloat("axis-OffsetX");
                }
                if (blockInfo.Data.HasKey("axis-OffsetY"))
                {
                    axis.OffsetY = blockInfo.Data.ReadFloat("axis-OffsetY");
                }
                if (blockInfo.Data.HasKey("axis-Min"))
                {
                    axis.Min = blockInfo.Data.ReadFloat("axis-Min");
                }
                if (blockInfo.Data.HasKey("axis-Max"))
                {
                    axis.Max = blockInfo.Data.ReadFloat("axis-Max");
                }
                if (blockInfo.Data.HasKey("axis-Lerp"))
                {
                    axis.Lerp = blockInfo.Data.ReadFloat("axis-Lerp");
                }
                //if (blockInfo.Data.HasKey("axis-Enable"))
                //{
                //    axis.Enable = blockInfo.Data.ReadBool("axis-Enable");
                //}

                return axis;
            }
            JoyAxis[] formatDataToJoyAxes(Modding.Blocks.BlockInfo blockInfo)
            {
                ModData modData = Mod.mod.GetComponent<ModDataManager>().ModData;
                var  axes = new JoyAxis[] { };
                int index = 0;

                if (blockInfo.Data.HasKey("BlockAxis-Number"))
                {
                    index = blockInfo.Data.ReadInt("BlockAxis-Number");
                }

                for (int i = 0; i < index; i++)
                {
                    var key = string.Format("JoyAxis {0}", i);

                    if (blockInfo.Data.HasKey(key))
                    {
                        var guid = new Guid(blockInfo.Data.ReadString(key));

                        JoyAxis joyAxis = modData.joyAxes.ToList().Find(match => match.Guid == guid);
                        if (joyAxis != null)
                        {
                            var axes_list = axes.ToList();
                            axes_list.Add(joyAxis);
                            axes = axes_list.ToArray();
                        }
                    }
                }
                return axes;
            }
        }

        public virtual void SafeAwake() { }
        public virtual void OnSimulateStart_Enable() { }
        public virtual void SimulateUpdateAlways_Enable() { }
        public virtual void SimulateFixedUpdate_Enable() { }
        public virtual void SimulateLateUpdate_Enable() { }
    }
}
