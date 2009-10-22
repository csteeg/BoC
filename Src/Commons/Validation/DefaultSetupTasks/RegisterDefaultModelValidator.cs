using System;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;

namespace BoC.Validation.DefaultSetupTasks
{
    public class RegisterDefaultModelValidator : IContainerInitializer
    {
        public void Execute()
        {
            if (!IoC.IsRegistered<IModelValidator>())
            {
                IoC.RegisterType<IModelValidator, DataAnnotationsModelValidator>();
            }
        }
    }
}