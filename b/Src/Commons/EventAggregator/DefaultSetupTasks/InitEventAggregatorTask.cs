using System;
using BoC.InversionOfControl;

namespace BoC.EventAggregator.DefaultSetupTasks
{
    public class InitEventAggregatorTask : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        public InitEventAggregatorTask(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            if (!dependencyResolver.IsRegistered<IEventAggregator>())
            {
                dependencyResolver.RegisterSingleton<IEventAggregator, EventAggregator>();
            }
        }
    }
}