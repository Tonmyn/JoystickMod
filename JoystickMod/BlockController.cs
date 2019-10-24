using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using Modding.Blocks;
using Block = Modding.Blocks.Block;
using JoystickMod.Blocks;

namespace JoystickMod
{
    public class BlockController : MonoBehaviour
    {

        public static Dictionary<int, Type> dic_AxisBlock = new Dictionary<int, Type>()
        {
         {(int)BlockType.SteeringHinge,typeof(SteeringScript) },
        };

        public Dictionary<Guid, JoyAxis> dic_AxisData = new Dictionary<Guid, JoyAxis>();

        private JoyAxis _copyAxisSource;

        private void Awake()
        {
            Events.OnMachineLoaded += load;
            Events.OnBlockInit += AddJoyAxis;
           
        }

        private void Update()
        {
            if (BlockMapper.CurrentInstance != null)
            {
                var block = BlockMapper.CurrentInstance.Block;
                if (isAxisBlock(block) && block != null)
                {
                    if (InputManager.CopyKeys())
                    {
                        _copyAxisSource = GetJoyAxisData(block);
                    }
                    if (InputManager.PasteKeys())
                    {
                        if (_copyAxisSource != null)
                        {
                            //block.GetComponent<JoystickMod.Block>().joyAxis.CopyProperties(_copyAxisSource);
                            block.GetComponent<JoystickMod.Block>().joyAxis.JoyIndex = _copyAxisSource.JoyIndex;
                            block.GetComponent<JoystickMod.Block>().joyAxis.AxisIndex = _copyAxisSource.AxisIndex;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Sensitivity = _copyAxisSource.Sensitivity;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Curvature = _copyAxisSource.Curvature;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Deadzone = _copyAxisSource.Deadzone;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Invert = _copyAxisSource.Invert;
                            block.GetComponent<JoystickMod.Block>().joyAxis.OffsetX = _copyAxisSource.OffsetX;
                            block.GetComponent<JoystickMod.Block>().joyAxis.OffsetY = _copyAxisSource.OffsetY;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Min = _copyAxisSource.Min;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Max = _copyAxisSource.Max;
                            block.GetComponent<JoystickMod.Block>().joyAxis.Lerp = _copyAxisSource.Lerp;
                        }
                    }
                }
            }      
        }


        private void AddJoyAxis(Modding.Blocks.Block block)
        {
            var blockID = block.InternalObject.BlockID;

            if (dic_AxisBlock.ContainsKey(blockID))
            {
                var com = (block.GameObject.GetComponent(dic_AxisBlock[blockID]) ?? block.GameObject.AddComponent(dic_AxisBlock[blockID])) as Block;

                if (dic_AxisData.ContainsKey(block.Guid))
                {
                    com.joyAxis = dic_AxisData[block.Guid];
                }
            }
        }

        private void load(PlayerMachineInfo info)
        {
            dic_AxisData.Clear();

            foreach (var v in info.Blocks)
            {
                if (dic_AxisBlock.ContainsKey(v.Type))
                {
                    var axis = formatDataToJoyAxis(v);
                    dic_AxisData.Add(v.Guid, axis);
                }
            }

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

                return axis;
            }

        }

        public static bool isAxisBlock(BlockBehaviour block)
        {
            if (dic_AxisBlock.ContainsKey(block.BlockID))
            {
                return true;
            }
            return false;
        }


        public static JoyAxis GetJoyAxisData(BlockBehaviour block)
        {
            try
            {
                return block.GetComponent<JoystickMod.Block>().joyAxis;
            }
            catch (Exception e)
            {
                BesiegeConsoleController.ShowMessage("Get Axis data is wrong");
                BesiegeConsoleController.ShowMessage(e.Message);
                return JoyAxis.Default;
            }
        }
    }
}
