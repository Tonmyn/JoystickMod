using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoystickMod.Blocks
{
    public class SteeringScript:Block
    {
        private SteeringWheel steeringWheel;

        private float max, min;

        private AnimationCurve animationCurve;

        public override void OnSimulateStart_Enable()
        {
            steeringWheel = BB.GetComponent<SteeringWheel>();
           

            if (/*steeringWheel.LimitsSlider.IsActive ||*/ steeringWheel.allowLimits)
            {
                if (steeringWheel.Flipped)
                {
                    animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*-1f**/joyAxis.Min, steeringWheel.LimitsSlider.Max), new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, -steeringWheel.LimitsSlider.Min) });
                }
                else
                {
                    animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*-1f * */joyAxis.Min, steeringWheel.LimitsSlider.Min), new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, -steeringWheel.LimitsSlider.Max) });
                }

            }
            else
            {
                animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*-1f*/joyAxis.Min, -180f), new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, 180f) });
            }

        }

        public override void SimulateUpdateAlways_Enable()
        {


            rigidbody.WakeUp();

            var value = 0f;
            float angleSpeed = Time.deltaTime * 100f * steeringWheel.SpeedSlider.Value * joyAxis.Lerp;

            value = Mathf.MoveTowards(steeringWheel.AngleToBe, animationCurve.Evaluate( joyAxis.CurveValue), angleSpeed);

            steeringWheel.AngleToBe = value;

        }
    }
}
