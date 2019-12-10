using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoystickMod.Blocks
{
    //class FlyingScript:Block
    //{
    //    FlyingController flyingController;

    //    public override void OnSimulateStart_Enable()
    //    {
    //        flyingController = GetComponent<FlyingController>();
    //    }

    //    public override void SimulateUpdateAlways_Enable()
    //    {
    //        Fly(Input.GetKey(KeyCode.F));

    //         void Fly(bool f)
    //        {
    //            if (f && !flyingController.isFrozen && flyingController.canFly)
    //            {
    //                flyingController.speedToGo = new Vector3(0, 0, 100f);
    //                flyingController.lerpSpeed = 6f;
    //                rigidbody.drag = 1.5f;
    //                flyingController.flying = true;
    //            }
    //            else
    //            {
    //                flyingController.speedToGo = Vector3.zero;
    //                rigidbody.drag = 0.5f;
    //                flyingController.flying = false;
    //            }
    //        }
    //    }

    //}
}
