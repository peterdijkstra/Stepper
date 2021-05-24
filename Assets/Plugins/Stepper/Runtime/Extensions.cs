using System.Collections;
using System.Collections.Generic;
using Stepper;
using UnityEngine;

public static class Extensions
{
	public static void UpdateStepData(this ref StepData stepData, in StepData previousStepData)
	{
		stepData.timeScale       = Time.timeScale;
		stepData.deltaTime       = Time.fixedDeltaTime;
		stepData.fixedTime       = Time.fixedTime;
		stepData.renderDeltaTime = Time.deltaTime;
		stepData.renderTime      = Time.time;
		stepData.alpha           = Time.deltaTime / Time.fixedDeltaTime;
		stepData.interpolatedSteps =
			(float)(stepData.steps * stepData.alpha + previousStepData.steps * (1.0f - stepData.alpha));
	}
}
