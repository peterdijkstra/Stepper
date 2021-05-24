using System;
using System.Collections;
using System.Collections.Generic;
using Stepper;
using UnityEngine;

public class MonoStepperParent : MonoBehaviour
{
    private readonly MainStepper _mainStepper = new MainStepper();

    private void FixedUpdate()
    {
        _mainStepper.CallFixedSteps();
    }

    private void Update()
    {
        _mainStepper.CallRenderStep();
    }

    private void LateUpdate()
    {
        _mainStepper.CallLateRenderStep();
    }
}
