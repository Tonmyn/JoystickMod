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

        private void Awake()
        {
          Events.OnMachineLoaded += load;
            Events.OnBlockInit += AddJoyAxis;
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
            Debug.Log("load");
            foreach (var v in info.Blocks)
            {
                if (dic_AxisBlock.ContainsKey(v.Type))
                {
                    Debug.Log("需要储存数据");

                    var axis = formatDataToJoyAxis(v);
                    dic_AxisData.Add(v.Guid, JoyAxis.Default);
                }
            }

            JoyAxis formatDataToJoyAxis(Modding.Blocks.BlockInfo blockInfo)
            {
                var axis = JoyAxis.Default;


                if (blockInfo.Data.HasKey("axis-test"))
                {
                    
                    axis.AxisIndex = blockInfo.Data.ReadInt("axis-test");
                    Debug.Log("read " + blockInfo.Data.ReadInt("axis-test"));
                }


                return axis;
            }

        }

    }
}
