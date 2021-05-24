using System;
using System.Collections.Generic;

namespace Stepper
{
	public delegate void OnStep(in StepData stepData);

	public interface IMainStepper
	{
		ISubStepper SubStepper { get; }
	}

	public interface IParentStepper
	{
		public void AddSubStepper(ISubStepper      subStepper);
		public void RemoveSubStepper(ISubStepper   subStepper);
		public bool ContainsSubStepper(ISubStepper subStepper);
	}

	public interface ISubStepper
	{
		event OnStep OnPreFixedStep;
		event OnStep OnFixedStep;
		event OnStep OnPostFixedStep;

		event OnStep OnRenderStep;
		event OnStep OnLateRenderStep;

		void PreFixedStep(in StepData stepData);

		void FixedStep(in StepData stepData);

		void PostFixedStep(in StepData stepData);

		void RenderStep(in StepData stepData);

		void LateRenderStep(in StepData stepData);

		IParentStepper ParentStepper { get; }

		void AddStep<TStep>(TStep    step) where TStep : IStep;
		void RemoveStep<TStep>(TStep step) where TStep : IStep;
		bool ContainsStep<TStep>(TStep    step) where TStep : IStep;
	}

	public class SubStepper : ISubStepper, IParentStepper
	{
		public StepperList<IPreFixedStep>  PreFixedStepList  { get; }
		public StepperList<IFixedStep>     FixedStepList     { get; }
		public StepperList<IPostFixedStep> PostFixedStepList { get; }

		public StepperList<IRenderStep>     RenderStepList     { get; }
		public StepperList<ILateRenderStep> LateRenderStepList { get; }

		private readonly List<ISubStepper>  _subSteppers;
		private readonly Queue<ISubStepper> _subStepperAddQueue;
		private readonly Queue<ISubStepper> _subStepperRemoveQueue;

		public event OnStep OnPreFixedStep;
		public event OnStep OnFixedStep;
		public event OnStep OnPostFixedStep;
		public event OnStep OnRenderStep;
		public event OnStep OnLateRenderStep;

		public IParentStepper ParentStepper { get; }

		public void AddStep<TStep>(TStep step) where TStep : IStep
		{
			switch (step)
			{
				case IPreFixedStep preFixedStep:
					PreFixedStepList.Add(preFixedStep);
					break;
				case IFixedStep fixedStep:
					FixedStepList.Add(fixedStep);
					break;
				case IPostFixedStep postFixedStep:
					PostFixedStepList.Add(postFixedStep);
					break;
				case IRenderStep renderStep:
					RenderStepList.Add(renderStep);
					break;
				case ILateRenderStep lateRenderStep:
					LateRenderStepList.Add(lateRenderStep);
					break;
			}
		}
		
		public void RemoveStep<TStep>(TStep step) where TStep : IStep
		{
			switch (step)
			{
				case IPreFixedStep preFixedStep:
					PreFixedStepList.Remove(preFixedStep);
					break;
				case IFixedStep fixedStep:
					FixedStepList.Remove(fixedStep);
					break;
				case IPostFixedStep postFixedStep:
					PostFixedStepList.Remove(postFixedStep);
					break;
				case IRenderStep renderStep:
					RenderStepList.Remove(renderStep);
					break;
				case ILateRenderStep lateRenderStep:
					LateRenderStepList.Remove(lateRenderStep);
					break;
			}
		}
		
		public bool ContainsStep<TStep>(TStep step) where TStep : IStep
		{
			return step switch
			{
				IPreFixedStep preFixedStep => PreFixedStepList.Contains(preFixedStep),
				IFixedStep fixedStep => FixedStepList.Contains(fixedStep),
				IPostFixedStep postFixedStep => PostFixedStepList.Contains(postFixedStep),
				IRenderStep renderStep => RenderStepList.Contains(renderStep),
				ILateRenderStep lateRenderStep => LateRenderStepList.Contains(lateRenderStep),
				_ => false
			};
		}

		public SubStepper(
			IEnumerable<IPreFixedStep>   preFixedSteps   = null,
			IEnumerable<IFixedStep>      fixedSteps      = null,
			IEnumerable<IPostFixedStep>  postFixedSteps  = null,
			IEnumerable<IRenderStep>     renderStep      = null,
			IEnumerable<ILateRenderStep> lateRenderSteps = null,
			IParentStepper               parentStepper   = null)
		{
			PreFixedStepList   = new StepperList<IPreFixedStep>(preFixedSteps     ?? new IPreFixedStep[0]);
			FixedStepList      = new StepperList<IFixedStep>(fixedSteps           ?? new IFixedStep[0]);
			PostFixedStepList  = new StepperList<IPostFixedStep>(postFixedSteps   ?? new IPostFixedStep[0]);
			RenderStepList     = new StepperList<IRenderStep>(renderStep          ?? new IRenderStep[0]);
			LateRenderStepList = new StepperList<ILateRenderStep>(lateRenderSteps ?? new ILateRenderStep[0]);

			_subStepperAddQueue    = new Queue<ISubStepper>();
			_subStepperRemoveQueue = new Queue<ISubStepper>();
			_subSteppers           = new List<ISubStepper>();

			ParentStepper = parentStepper;
			ParentStepper?.AddSubStepper(this);
		}

		public void PreFixedStep(in StepData stepData)
		{
			while (_subStepperRemoveQueue.Count > 0)
				_subSteppers.Remove(_subStepperRemoveQueue.Dequeue());

			while (_subStepperAddQueue.Count > 0)
				_subSteppers.Add(_subStepperAddQueue.Dequeue());

			PreFixedStepList.Step(in stepData);
			OnPreFixedStep?.Invoke(in stepData);

			foreach (var stepper in _subSteppers)
				stepper.PreFixedStep(in stepData);
		}

		public void FixedStep(in StepData stepData)
		{
			FixedStepList.Step(in stepData);
			OnFixedStep?.Invoke(in stepData);

			foreach (var stepper in _subSteppers)
				stepper.FixedStep(in stepData);
		}

		public void PostFixedStep(in StepData stepData)
		{
			PostFixedStepList.Step(in stepData);
			OnPostFixedStep?.Invoke(in stepData);

			foreach (var stepper in _subSteppers)
				stepper.PostFixedStep(in stepData);
		}

		public void RenderStep(in StepData stepData)
		{
			RenderStepList.Step(in stepData);
			OnRenderStep?.Invoke(in stepData);

			foreach (var stepper in _subSteppers)
				stepper.RenderStep(in stepData);
		}

		public void LateRenderStep(in StepData stepData)
		{
			LateRenderStepList.Step(in stepData);
			OnLateRenderStep?.Invoke(in stepData);

			foreach (var stepper in _subSteppers)
				stepper.LateRenderStep(in stepData);
		}

		public void AddSubStepper(ISubStepper subStepper)
		{
			_subStepperAddQueue.Enqueue(subStepper);
		}

		public void RemoveSubStepper(ISubStepper subStepper)
		{
			_subStepperRemoveQueue.Enqueue(subStepper);
		}

		public bool ContainsSubStepper(ISubStepper subStepper)
		{
			return _subStepperAddQueue.Contains(subStepper) || _subSteppers.Contains(subStepper);
		}
	}
}