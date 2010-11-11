using System;
using BoC.InversionOfControl;

namespace BoC.Validation.DefaultSetupTasks
{
    public class RegisterDefaultModelValidator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        public RegisterDefaultModelValidator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            if (!dependencyResolver.IsRegistered<IModelValidator>())
            {
                dependencyResolver.RegisterType<IModelValidator, DataAnnotationsModelValidator>();
            }
        }
    }
}