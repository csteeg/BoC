using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.GlassConfig;
using Glass.Mapper.Configuration;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{
    public class RegisterConfigLoader: IContainerInitializer
    {
        private readonly IDependencyResolver _resolver;

        public RegisterConfigLoader(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Execute()
        {
            _resolver.RegisterType<IConfigurationLoader, AppDomainHelperConfigLoader>();
        }
    }
    
}
