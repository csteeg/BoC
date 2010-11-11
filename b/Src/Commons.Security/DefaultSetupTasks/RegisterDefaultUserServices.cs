using BoC.InversionOfControl;
using BoC.Security.Services;

namespace BoC.Security.DefaultSetupTasks
{
    public class RegisterDefaultUserServices : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public RegisterDefaultUserServices(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            if (!dependencyResolver.IsRegistered<IUserService>())
            {
                dependencyResolver.RegisterType<IUserService, DefaultUserService>();
            }
        }
    }
}