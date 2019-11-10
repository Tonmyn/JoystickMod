using JoystickMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class WaterCannonScript : Block
{
    AnimationCurve animationCurve;

    bool StrengthSliderValueSign = false;

    WaterCannonController waterCannonController;
    public override void OnSimulateStart_Enable()
    {
        waterCannonController = GetComponent<WaterCannonController>();

        animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f + joyAxis.CurveValue, 0f), new Keyframe(1f * joyAxis.Max, waterCannonController.StrengthSlider.Value) });

        StrengthSliderValueSign = waterCannonController.StrengthSlider.Value > 0 ? true : false;
    }
    float targetStrength = 0f;
    public override void SimulateUpdateAlways_Enable()
    {
        if (StrengthSliderValueSign)
        {
            waterCannonController.isActive = joyAxis.DirectionValue > 0 ? true : false;
        }

        var value = 0f;
        //if (LerpToggle.IsActive)
        //{
        value = Mathf.MoveTowards(targetStrength, animationCurve.Evaluate(joyAxis.CurveValue), joyAxis.Lerp/* * Mathf.Pow(10, (waterCannonController.StrengthSlider.Value.ToString().Length - 1))*/);
        //}
        //else
        //{
        //    value =/* animationCurve.Evaluate(*/joyAxis.Value/*)*/;
        //}
        targetStrength = value;

        waterCannonController.StrengthSlider.Value = targetStrength;
        waterCannonController.ParentMachine.RegisterFixedUpdate(waterCannonController, false);
    }

}

