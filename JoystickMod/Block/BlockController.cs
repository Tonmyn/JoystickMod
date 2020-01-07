using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;
using Modding.Blocks;
using JoystickMod.Blocks;

namespace JoystickMod
{
    public class BlockController : MonoBehaviour
    {
        public static Dictionary<int, Type> dic_AxisBlock = new Dictionary<int, Type>()
        {
         {(int)BlockType.SteeringHinge,typeof(SteeringScript) },
         {(int)BlockType.SteeringBlock,typeof(SteeringScript) },
           {(int)BlockType.Piston,typeof(PistonScript) },
            {(int)BlockType.Wheel,typeof(CogScript) },
             {(int)BlockType.LargeWheel,typeof(CogScript) },
             {(int)BlockType.CogMediumPowered,typeof(CogScript) },
             {(int)BlockType.WaterCannon,typeof(WaterCannonScript) },
             {(int)BlockType.FlyingBlock,typeof(FlyingScript) },
        };

        public Dictionary<Guid, Modding.Blocks.BlockInfo> dic_BlockInfo = new Dictionary<Guid, Modding.Blocks.BlockInfo>();

        private Block _copyBlockSource;

        private void Awake()
        {
            Events.OnBlockInit += AddJoyAxis;
            Events.OnMachineLoaded += OnMachineLoad;
        }
        private void Update()
        {
            if (BlockMapper.CurrentInstance != null  && !StatMaster.levelSimulating)
            {
                var block = BlockMapper.CurrentInstance.Block;
                if (isAxisBlock(block) && block != null)
                {
                    if (InputManager.CopyKeys())
                    {
                        _copyBlockSource = block.GetComponent<Block>();
                    }
                    if (InputManager.PasteKeys())
                    {
                        if (_copyBlockSource != null)
                        {
                            Block axisBlock = block.GetComponent<Block>();
                            axisBlock.AxisEnabled = _copyBlockSource.AxisEnabled;
                            axisBlock.Invert = _copyBlockSource.Invert;

                            List<JoyAxis> joyAxes = new List<JoyAxis>();
                            _copyBlockSource.joyAxes.ToList().ForEach((axis) => joyAxes.Add(axis.Copy()));
                            axisBlock.joyAxes = joyAxes.ToArray();
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

                if (dic_BlockInfo.ContainsKey(block.Guid))
                {
                    com.OnLoad(dic_BlockInfo[block.Guid]);     
                }
            }
        }
        private void OnMachineLoad(PlayerMachineInfo info)
        {
            dic_BlockInfo.Clear();

            foreach (var v in info.Blocks)
            {
                if (dic_AxisBlock.ContainsKey(v.Type))
                {
                    dic_BlockInfo.Add(v.Guid, v);
                }
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
        public static JoyAxis[] GetJoyAxisData(BlockBehaviour block)
        {
            try
            {
                return block.GetComponent<JoystickMod.Block>().joyAxes;
            }
            catch (Exception e)
            {
                BesiegeConsoleController.ShowMessage("Get Axis data is wrong");
                BesiegeConsoleController.ShowMessage(e.Message);
                return new JoyAxis[] { JoyAxis.Default };
            }
        }
    }
}
