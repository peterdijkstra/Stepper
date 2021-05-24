using System;

namespace Stepper
{
	public struct StepData : IEquatable<StepData>
	{
		public uint steps;
#if STEPPER_USE_DOUBLE
		public double deltaTime;
		public double renderDeltaTime;
		public double timeScale;
		public double alpha;
		public double interpolatedSteps;
		public double fixedTime;
		public double renderTime;
#else
		public float deltaTime;
		public float renderDeltaTime;
		public float timeScale;
		public float alpha;
		public float interpolatedSteps;
		public float fixedTime;
		public float renderTime;
#endif

		public bool Every(uint frame) => steps % frame == 0;

		public bool Equals(StepData other)
		{
			return steps == other.steps && deltaTime.Equals(other.deltaTime) && renderDeltaTime.Equals(other.renderDeltaTime) && timeScale.Equals(other.timeScale) && alpha.Equals(other.alpha) && interpolatedSteps.Equals(other.interpolatedSteps) && fixedTime.Equals(other.fixedTime) && renderTime.Equals(other.renderTime);
		}

		public override bool Equals(object obj)
		{
			return obj is StepData other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int)steps;
				hashCode = (hashCode * 397) ^ deltaTime.GetHashCode();
				hashCode = (hashCode * 397) ^ renderDeltaTime.GetHashCode();
				hashCode = (hashCode * 397) ^ timeScale.GetHashCode();
				hashCode = (hashCode * 397) ^ alpha.GetHashCode();
				hashCode = (hashCode * 397) ^ interpolatedSteps.GetHashCode();
				hashCode = (hashCode * 397) ^ fixedTime.GetHashCode();
				hashCode = (hashCode * 397) ^ renderTime.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return
				$"StepData (deltaTime: {deltaTime}, renderDeltaTime: {renderDeltaTime}, timeScale: {timeScale}, alpha: {alpha}, interpolatedSteps: {interpolatedSteps}, fixedTime: {fixedTime}, renderTime: {renderTime})";
		}
	}
}