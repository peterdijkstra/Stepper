using System.Collections;
using System.Collections.Generic;
using Stepper;
using Stepper.VContainer;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Stepper.VContainer
{
	public static class ContainerBuilderExtensions
	{
		public static void RegisterMainStepper<TStepper>(this IContainerBuilder builder,
			Lifetime lifetime = Lifetime.Singleton) where TStepper : MainStepper
		{
			builder.Register<TStepper>(lifetime).As<IParentStepper>().AsSelf();
		}

		public static void RegisterVMainStepper(this IContainerBuilder builder, Lifetime lifetime = Lifetime.Singleton)
		{
			builder.Register<VMainStepper>(lifetime).AsImplementedInterfaces().AsSelf();
		}

		public static void RegisterStepper<TStepper>(this IContainerBuilder builder, IParentStepper parentStepper = null, Lifetime lifetime = Lifetime.Scoped)
			where TStepper : ISubStepper
		{
			builder.Register<TStepper>(lifetime).As<ISubStepper>()
				.WithParameter(typeof(IParentStepper), parentStepper);
		}

		public static void RegisterStepperAsParent<TStepper>(this IContainerBuilder builder,
			IParentStepper parentStepper = null) where TStepper : ISubStepper
		{
			builder.Register<TStepper>(Lifetime.Scoped).As<ISubStepper, IParentStepper>()
				.WithParameter(typeof(IParentStepper), parentStepper);
		}
	}
}
