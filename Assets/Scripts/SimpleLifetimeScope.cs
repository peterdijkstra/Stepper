using Stepper;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SimpleLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // builder.Register<StepperParent>(Lifetime.Singleton);
        //
        // builder.Register<SubStepper>(Lifetime.Scoped);
        //
        builder.RegisterEntryPoint<Aaa>();
    }
}

public class Aaa : IPostStartable
{
    // private readonly SubStepper _stepper;
    //
    // public Aaa(SubStepper stepper)
    // {
    //     _stepper = stepper;
    // }
    
    public void PostStart()
    {
        Debug.Log(new SubStepper());
    }
}