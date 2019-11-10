﻿using System;
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
                animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(joyAxis.Min, steeringWheel.LimitsSlider.Min), new Keyframe(joyAxis.CurveValue, 0f), new Keyframe(joyAxis.Max, -steeringWheel.LimitsSlider.Max) });
            }
            else
            {
                animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(joyAxis.Min, steeringWheel.SpeedSlider.Value), new Keyframe(joyAxis.CurveValue, 0f), new Keyframe(joyAxis.Max, steeringWheel.SpeedSlider.Value) });
            }


        }

        public override void SimulateUpdateAlways_Enable()
        {


            rigidbody.WakeUp();
            if (steeringWheel.allowLimits && steeringWheel.LimitsSlider.IsActive)
            {
                var value = 0f;
                float angleSpeed = Time.deltaTime * 100f * steeringWheel.SpeedSlider.Value * joyAxis.Lerp;
                float targetValue = steeringWheel.Flipped ? -animationCurve.Evaluate(joyAxis.CurveValue) : animationCurve.Evaluate(joyAxis.CurveValue);

                value = Mathf.MoveTowards(steeringWheel.AngleToBe, targetValue, angleSpeed);
                steeringWheel.AngleToBe = value;
            }
            else
            {
                float angleSpeed = animationCurve.Evaluate(joyAxis.CurveValue) * joyAxis.Lerp;
                steeringWheel.AngleToBe += joyAxis.DirectionValue * Time.deltaTime * 100f * steeringWheel.targetAngleSpeed * (steeringWheel.Flipped ? 1 : -1) * angleSpeed;

            }
        }
    }
}
