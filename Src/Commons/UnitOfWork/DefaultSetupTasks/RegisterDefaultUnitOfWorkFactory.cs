using System;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;
using BoC.Validation;

namespace BoC.UnitOfWork.DefaultSetupTasks
{
    public class RegisterDefaultUnitOfWorkFactory : IContainerInitializer
    {
        public void Execute()
        {
            if (!IoC.IsRegistered<IUnitOfWorkFactory>())
            {
                IoC.RegisterType<IUnitOfWorkFactory, IoCUnitOfWorkFactory>();
            }
        }
    }
}