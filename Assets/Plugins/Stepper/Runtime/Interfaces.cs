namespace Stepper
{
	public interface IStep
	{
		void Step(in StepData stepData);
	}

	public interface IPreFixedStep : IStep
	{
		//void PreFixedStep(in StepData stepData);
	}
	
	public interface IFixedStep : IStep
	{
		//void FixedStep(in StepData stepData);
	}
	
	public interface IPostFixedStep : IStep
	{
		//void PostFixedStep(in StepData stepData);
	}
	
	public interface IRenderStep : IStep
	{
		//void RenderStep(in StepData stepData);
	}

	public interface ILateRenderStep : IStep
	{
		//void LateRenderStep(in StepData stepData);
	}

	// public interface IStepperPaused
	// {
	// 	void StepperPaused(bool paused);
	// }
}

