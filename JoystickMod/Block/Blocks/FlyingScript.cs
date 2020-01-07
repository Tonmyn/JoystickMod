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
        float f = 85f;
        public override void OnSimulateStart_Enable()
        {
            flyingController = BB.GetComponent<FlyingController>();

        }

        public override void SimulateUpdateAlways_Enable()
        {
            if (GetAxesValue() != 0)
            {
                flyingController.spinObj.Rotate(new Vector3(0, 0, -1000) * Time.deltaTime);

                force = 100f * flyingController.SpeedSlider.Value;
            }
            else
            {
                force =f;
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

            flyingController.Rigidbody.AddRelativeForce(vector);
            flyingController.Rigidbody.AddForce(-(flyingController.Rigidbody.velocity * flyingController.dragScaler));
        }

    }
}
