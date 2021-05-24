using System.Collections.Generic;
using VContainer.Unity;

namespace Stepper.VContainer
{
	public class VMainStepper : IMainStepper, IParentStepper, IFixedTickable, ITickable, ILateTickable
	{
		private StepData _stepData;

		public StepData StepData => _stepData;

		private StepData _previousStepData;

		private readonly SubStepper _subStepper;

		public ISubStepper    SubStepper    => _subStepper;
		public IParentStepper ParentStepper => _subStepper;

		public VMainStepper(
			IEnumerable<IPreFixedStep>   preFixedSteps   = null,
			IEnumerable<IFixedStep>      fixedSteps      = null,
			IEnumerable<IPostFixedStep>  postFixedSteps  = null,
			IEnumerable<IRenderStep>     renderStep      = null,
			IEnumerable<ILateRenderStep> lateRenderSteps = null,
			IEnumerable<ISubStepper>     steppers        = null)
		{
			_subStepper = new SubStepper(preFixedSteps, fixedSteps, postFixedSteps, renderStep, lateRenderSteps);
			
			if (steppers == null)
				return;

			foreach (var stepper in steppers)
				AddSubStepper(stepper);
		}

		public void CallFixedSteps()
		{
			_previousStepData = _stepData;
			_stepData.steps++;

			_stepData.UpdateStepData(_previousStepData);

			SubStepper.PreFixedStep(in _stepData);
			SubStepper.FixedStep(in _stepData);
			SubStepper.PostFixedStep(in _stepData);
		}

		public void CallRenderStep()
		{
			_stepData.UpdateStepData(_previousStepData);
			SubStepper.RenderStep(in _stepData);
		}

		public void CallLateRenderStep()
		{
			_stepData.UpdateStepData(_previousStepData);
			SubStepper.LateRenderStep(in _stepData);
		}

		public void AddSubStepper(ISubStepper subStepper) => ParentStepper.AddSubStepper(subStepper);

		public void RemoveSubStepper(ISubStepper subStepper) => ParentStepper.RemoveSubStepper(subStepper);

		public bool ContainsSubStepper(ISubStepper subStepper) => ParentStepper.ContainsSubStepper(subStepper);

		public void FixedTick()
		{
			CallFixedSteps();
		}

		public void Tick()
		{
			CallRenderStep();
		}

		public void LateTick()
		{
			CallLateRenderStep();
		}
	}
}