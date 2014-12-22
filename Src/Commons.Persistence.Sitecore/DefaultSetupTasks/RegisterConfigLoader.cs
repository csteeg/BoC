using System.Linq;
using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.DefaultSetupTasks;
using BoC.Persistence.SitecoreGlass.GlassConfig;
using BoC.Tasks;
using Glass.Mapper.Configuration;
using Glass.Mapper.Sc.CastleWindsor;

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
