using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;
using BoC.Security.Services;

namespace BoC.Security.DefaultSetupTasks
{
    public class RegisterDefaultUserServices : IContainerInitializer
    {
        public void Execute()
        {
            if (!IoC.IsRegistered<IUserService>())
            {
                IoC.RegisterType<IUserService, DefaultUserService>();
            }
        }
    }
}