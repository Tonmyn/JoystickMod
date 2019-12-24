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
    float baseStrength = 1f;
    WaterCannonController waterCannonController;
    public override void OnSimulateStart_Enable()
    {
        waterCannonController = GetComponent<WaterCannonController>();

        animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(CurveMax, waterCannonController.StrengthSlider.Value) });

        StrengthSliderValueSign = waterCannonController.StrengthSlider.Value > 0 ? true : false;
        baseStrength = Mathf.Abs(waterCannonController.StrengthSlider.Value);
    }
    float targetStrength = 0f;
    public override void SimulateUpdateAlways_Enable()
    {
        if (StrengthSliderValueSign)
        {
            waterCannonController.isActive = GetAxesValueDirection() > 0 ? true : false;
        }

        var value = 0f;
        value = Mathf.MoveTowards(targetStrength, animationCurve.Evaluate(GetAxesValue()), Lerp * Mathf.Pow(Mathf.Min(baseStrength * 0.1f, 10f), (waterCannonController.StrengthSlider.Value.ToString().Length - 1)));

        targetStrength = value;

        waterCannonController.StrengthSlider.Value = targetStrength;
        waterCannonController.ParentMachine.RegisterFixedUpdate(waterCannonController, false);
    }

}

