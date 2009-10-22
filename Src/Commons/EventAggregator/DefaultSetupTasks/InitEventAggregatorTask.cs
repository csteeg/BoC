using System;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;

namespace BoC.EventAggregator.DefaultSetupTasks
{
    public class InitEventAggregatorTask : IContainerInitializer
    {
        public void Execute()
        {
            if (!IoC.IsRegistered<IEventAggregator>())
            {
                IoC.RegisterSingleton<IEventAggregator, EventAggregator>();
            }
        }
    }
}