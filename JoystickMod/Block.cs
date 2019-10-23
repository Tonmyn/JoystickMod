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

            joyAxis = loadJoyAxis();


        }


        private void load(PlayerMachineInfo info )
        {
            Debug.Log("load");
        }

        private JoyAxis loadJoyAxis()
        {
            //foreach (var v in BB.InitialState.)
            //{
            //    Debug.Log(v);
            //}
            Debug.Log(BB.InitializedState);


            Machine.Active().MachineData.Write("test", "test");

            return JoyAxis.Default;
        }

        private void Update()
        {
        }

        public void OnSave(PlayerMachineInfo playerMachineInfo)
        {


            //var info = Modding.Blocks.BlockInfo.From(Modding.Blocks.Block.From(BB)); ;
            //info.Data.Write("axis-test", "test");

            foreach (var v in playerMachineInfo.Blocks)
            {
                if (v.Guid == BB.Guid)
                {
                    int i = 1;
                    v.Data.Write("axis-test", joyAxis.AxisIndex);
                    break;
                }
            }




            Debug.Log("On Save " + BB.Guid);
        }

        private void OnDestroy()
        {
            Debug.Log("destroy");

            //Events.OnMachineLoaded -= load;
            Events.OnMachineSave -= OnSave;
        }
    }
}
