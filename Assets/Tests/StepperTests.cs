using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Stepper;
using Stepper.VContainer;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;

public class MockSteps
{
	public abstract class BaseMock
	{
		public uint Steps { get; protected set; }
	}

	public class PreFixedMock : BaseMock, IPreFixedStep
	{
		public void Step(in StepData stepData)
		{
			Steps++;
			Debug.Log($"PreFixedMock. {stepData}");
		}
	}

	public class FixedMock : BaseMock, IFixedStep
	{
		public void Step(in StepData stepData)
		{
			Steps++;
			Debug.Log($"FixedMock. {stepData}");
		}
	}

	public class PostFixedMock : BaseMock, IPostFixedStep
	{
		public void Step(in StepData stepData)
		{
			Steps++;
			Debug.Log($"PostFixedMock. {stepData}");
		}
	}

	public class RenderMock : BaseMock, IRenderStep
	{
		public void Step(in StepData stepData)
		{
			Steps++;
			Debug.Log($"RenderMock. {stepData}");
		}
	}

	public class LateRenderMock : BaseMock, ILateRenderStep
	{
		public void Step(in StepData stepData)
		{
			Steps++;
			Debug.Log($"LateRenderMock. {stepData}");
		}
	}
}

public class StepperTests
{
	[Test]
	public void AddToStepper()
	{
		var stepperParent = new MainStepper();

		var fixedMock = new MockSteps.FixedMock();

		stepperParent.SubStepper.AddStep(fixedMock);

		Assert.That(stepperParent.SubStepper.ContainsStep(fixedMock));
	}

	[Test]
	public void IncrementTime()
	{
		var stepperParent = new MainStepper();
		Assert.That(stepperParent.StepData.steps == 0);
		stepperParent.CallFixedSteps();
		Assert.That(stepperParent.StepData.steps == 1);
	}

	[Test]
	public void CallMock()
	{
		var stepperParent = new MainStepper();

		var fixedMock = new MockSteps.FixedMock();

		stepperParent.SubStepper.AddStep(fixedMock);

		Assert.That(fixedMock.Steps == 0);

		stepperParent.CallFixedSteps();

		Assert.That(fixedMock.Steps == 1);
	}

	[Test]
	public void AddStepperChild()
	{
		var stepperParent = new MainStepper();

		var stepperChild = new SubStepper();

		stepperParent.AddSubStepper(stepperChild);

		Assert.That(stepperParent.ContainsSubStepper(stepperChild));
	}

	[Test]
	public void CallMockInChild()
	{
		var stepperParent = new MainStepper();
		var stepperChild  = new SubStepper();
		stepperParent.AddSubStepper(stepperChild);

		var fixedMock = new MockSteps.FixedMock();

		stepperChild.FixedStepList.Add(fixedMock);

		Assert.That(fixedMock.Steps == 0);

		stepperParent.CallFixedSteps();

		Assert.That(fixedMock.Steps == 1);
	}

	[Test]
	public void tttt()
	{
		var scope = LifetimeScope.Create(builder =>
		{
			builder.Register<ISubStepper, SubStepper>(Lifetime.Scoped);
		});

		Debug.Log(scope.Container.Resolve<ISubStepper>());
	}

	[Test]
	public void VContainerTest()
	{
		var scope = LifetimeScope.Create(builder =>
		{
			// var stepperParent = new StepperParent();

			for (int i = 0; i < 10; i++)
			{
				builder.Register<MockSteps.FixedMock>(Lifetime.Scoped).AsImplementedInterfaces();
			}

			builder.RegisterVMainStepper();

			// builder.RegisterInstance(stepperParent);
			// builder.RegisterInstance<StepperChild>(stepperParent);
		});

		var stepperParent = scope.Container.Resolve<IParentStepper>();

		Debug.Log($"logging parent {stepperParent}");
		var vMainStepper = (VMainStepper)stepperParent;
		var subs         = (SubStepper)vMainStepper.SubStepper;
		foreach (var fixedStep in subs.FixedStepList.Current)
		{
			Debug.Log(fixedStep);
		}

		Debug.Log("done logging parent");

		Debug.Log(scope);

		LifetimeScope childScope;
		childScope = scope.CreateChild(builder =>
		{
			builder.Register<MockSteps.RenderMock>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();


			var parent = scope.Container.Resolve<IParentStepper>();
			builder.RegisterStepper<SubStepper>(parent);
			//builder.Register<SubStepper>(Lifetime.Scoped).As<ISubStepper, IParentStepper>().WithParameter(typeof(IParentStepper), scope.Container.Resolve<MainStepper>());
		});

		var stepperChild = childScope.Container.Resolve<ISubStepper>();

		Debug.Log($"logging child {stepperChild}");
		foreach (var fixedStep in ((SubStepper)stepperChild).RenderStepList.Current)
		{
			Debug.Log(fixedStep);
		}

		Debug.Log($"done logging child");

		Assert.That(((VMainStepper)stepperParent).SubStepper != stepperChild);
		Assert.That(stepperChild.ParentStepper               == stepperParent);

		vMainStepper.FixedTick();
		vMainStepper.Tick();
		
		Assert.That(childScope.Container.Resolve<MockSteps.RenderMock>().Steps == 1);
	}
}