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

        public override void OnSimulateStart_Enable()
        {
            flyingController = BB.GetComponent<FlyingController>();

        }

        public override void SimulateUpdateAlways_Enable()
        {
            if (GetAxesValue() != 0)
            {
                flyingController.flying = true;
            }
        }

        public override void SimulateFixedUpdateAlways_Enable()
        {
            if (BB.noRigidbody)
            {
                enabled = false;
                return;
            }
            var force = 100f * flyingController.SpeedSlider.Value;
            var vector = new Vector3(0, force - (flyingController.Rigidbody.velocity * flyingController.dragScaler).magnitude, 0);

            flyingController.Rigidbody.AddRelativeForce(vector);
        }

    }
}
