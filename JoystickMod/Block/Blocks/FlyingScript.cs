using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoystickMod.Blocks
{
    class FlyingScript : Block
    {
        FlyingController flyingController;

        float force = 0f;
        float f = 100f;
        public override void OnSimulateStart_Enable()
        {
            flyingController = BB.GetComponent<FlyingController>();
            f = BB.BuildingBlock.GetComponent<FlyingScript>().f;
        }

        public override void SimulateUpdateAlways_Enable()
        {
            if (GetAxesValue() != 0)
            {
                if (GetAxesValueDirection() != 0f)
                {
                    flyingController.spinObj.Rotate(new Vector3(0, 0, -1000) * Time.deltaTime);
                }
                   
                force = 100f * flyingController.SpeedSlider.Value * GetAxesValue();
                flyingController.Rigidbody.drag = 1.5f;
            }
            else
            {
                force =0;
                flyingController.Rigidbody.drag = 0.5f;
            }
        }

        public override void SimulateFixedUpdateAlways_Enable()
        {
            if (BB.noRigidbody || flyingController.isFrozen)
            {
                enabled = false;
                return;
            }
            
            var vector = new Vector3(0, 0,force);
       
            flyingController.Rigidbody.AddForce(transform.TransformVector(vector) - (flyingController.Rigidbody.velocity * flyingController.dragScaler));
        }

    }
}
