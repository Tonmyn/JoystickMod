using JoystickMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class PistonScript : Block
{
    ConfigurableJoint myJoint;
    AnimationCurve animationCurve;


    SliderCompress sliderCompress;
    public override void OnSimulateStart_Enable()
    {
        myJoint = GetComponent<ConfigurableJoint>();
        sliderCompress = GetComponent<SliderCompress>();

        animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f + joyAxis.CurveValue, sliderCompress.startLimit), new Keyframe(/*-1f * joyAxis.Sign*/1f*joyAxis.Max, sliderCompress.newLimit) });
    }
    float targetPos = 0f;
    public override void SimulateLateUpdate_Enable()
    {
        var value = 0f;

        value = Mathf.MoveTowards(targetPos, animationCurve.Evaluate(joyAxis.CurveValue), joyAxis.Lerp * sliderCompress.SpeedSlider.Value * Time.deltaTime * 6f);

        targetPos = value;
        sliderCompress.posToBe = targetPos;
        float single = myJoint.targetPosition.x;
        if (single != sliderCompress.posToBe && myJoint.connectedBody != null)
        {
            if (rigidbody.IsSleeping())
            {
                rigidbody.WakeUp();
            }
            if (this.myJoint.connectedBody.IsSleeping())
            {
                this.myJoint.connectedBody.WakeUp();
            }
            ConfigurableJoint vector3 = myJoint;
            //float single1 = Mathf.Lerp(single, sliderCompress.posToBe, Time.deltaTime * sliderCompress.lerpSpeed * sliderCompress.SpeedSlider.Value * Mathf.Abs(sliderCompress.newLimit));
            float single1 = targetPos;
            float single2 = myJoint.targetPosition.y;
            float single3 = myJoint.targetPosition.z;
            vector3.targetPosition = new Vector3(single1, single2, single3);
        }

    }
}

