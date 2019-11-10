using JoystickMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CogScript : Block
{

    Rigidbody Rigidbody;
    ConfigurableJoint myJoint;
    AnimationCurve animationCurve;

    CogMotorControllerHinge cogMotorControllerHinge;
    public override void OnSimulateStart_Enable()
    {
        Rigidbody = GetComponent<Rigidbody>();
        myJoint = GetComponent<ConfigurableJoint>();
        cogMotorControllerHinge = GetComponent<CogMotorControllerHinge>();

       animationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f+joyAxis.CurveValue, 0f), new Keyframe(1f*joyAxis.Max, cogMotorControllerHinge.SpeedSlider.Value) });
    }

    public override void SimulateUpdateAlways_Enable()
    {
        cogMotorControllerHinge.Input = joyAxis.DirectionValue;
    }
    float targetSpeed = 0f;
    public override void SimulateFixedUpdate_Enable()
    {

        //if (Rigidbody.IsSleeping())
        //{
        //    Rigidbody.WakeUp();
        //}
        //try
        //{
        //    if (myJoint.connectedBody.IsSleeping())
        //    {
        //        myJoint.connectedBody.WakeUp();
        //    }
        //}
        //catch { }

        float value = 0f;

        value = Mathf.MoveTowards(targetSpeed, animationCurve.Evaluate(Mathf.Abs(joyAxis.CurveValue)), joyAxis.Lerp * Time.deltaTime);

        targetSpeed = value;
        cogMotorControllerHinge.SpeedSlider.Value = targetSpeed;

    }

}

