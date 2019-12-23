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


            //if (steeringWheel.allowLimits)
            //{
            //    if (steeringWheel.Flipped)
            //    {
            //        animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*-1f**/joyAxis.Min, steeringWheel.LimitsSlider.Max), new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, -steeringWheel.LimitsSlider.Min) });
            //    }
            //    else
            //    {
            //        animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*-1f * */joyAxis.Min, steeringWheel.LimitsSlider.Min), new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, -steeringWheel.LimitsSlider.Max) });
            //    }

            //}
            //else
            //{
            //    animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*-1f*/joyAxis.Min, -180f), new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, 180f) });
            //}
            //if (steeringWheel.Flipped)
            //{
            //    animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(joyAxis.Min, steeringWheel.LimitsSlider.Max), new Keyframe(joyAxis.CurveValue, 0f), new Keyframe(joyAxis.Max, -steeringWheel.LimitsSlider.Min) });

            //}
            //else
            //{

            //}

            if (steeringWheel.allowLimits && steeringWheel.LimitsSlider.IsActive)
            {
                animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*joyAxis.Min*/CurveMin, steeringWheel.LimitsSlider.Min), new Keyframe(/*joyAxis.CurveValue*/0f, 0f), new Keyframe(/*joyAxis.Max*/CurveMax, -steeringWheel.LimitsSlider.Max) });
            }
            else
            {
                animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(/*joyAxis.Min*/CurveMin, steeringWheel.SpeedSlider.Value), new Keyframe(/*joyAxis.CurveValue*/0f, 0f), new Keyframe(/*joyAxis.Max*/CurveMax, steeringWheel.SpeedSlider.Value) });
            }


        }

        public override void SimulateUpdateAlways_Enable()
        {
            float axesValue = GetAxesValue();

            rigidbody.WakeUp();
            if (steeringWheel.allowLimits && steeringWheel.LimitsSlider.IsActive)
            {
                var value = 0f;
                float angleSpeed = Time.deltaTime * 100f * steeringWheel.SpeedSlider.Value * Lerp;
                float targetValue = steeringWheel.Flipped ? -animationCurve.Evaluate(axesValue) : animationCurve.Evaluate(axesValue);

                value = Mathf.MoveTowards(steeringWheel.AngleToBe, targetValue, angleSpeed);
                steeringWheel.AngleToBe = value;
            }
            else
            {
                float angleSpeed = animationCurve.Evaluate(axesValue) * Lerp;
                steeringWheel.AngleToBe +=/* joyAxis.DirectionValue*/GetAxesValueDirection() * Time.deltaTime * 100f * steeringWheel.targetAngleSpeed * (steeringWheel.Flipped ? 1 : -1) * angleSpeed;

            }
        }
    }
}
