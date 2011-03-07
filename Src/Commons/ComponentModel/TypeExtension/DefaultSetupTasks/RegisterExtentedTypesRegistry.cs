using BoC.InversionOfControl;

namespace BoC.ComponentModel.TypeExtension.DefaultSetupTasks
{
    public class RegisterExtentedTypesRegistry : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public RegisterExtentedTypesRegistry(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            if (!(dependencyResolver.IsRegistered(typeof(IExtendedTypesRegistry))))
            {
                dependencyResolver.RegisterSingleton<IExtendedTypesRegistry, ExtendedTypesRegistry>();
            }
        }

   }
}