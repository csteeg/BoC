using System.Linq;
using BoC.InversionOfControl;
using BoC.Tasks;
using Castle.MicroKernel.Registration;
using Glass.Mapper.Configuration;
using Glass.Mapper.Pipelines.ObjectConstruction;
using Glass.Mapper.Sc.CastleWindsor;
using Glass.Mapper.Sc.CodeFirst;
using Glass.Mapper.Sc.ContentSearch.Pipelines.ObjectConstruction.Tasks.SearchProxy;
using Sitecore.SecurityModel;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{
    public class InitGlass : IBootstrapperTask
    {
        private readonly IDependencyResolver _resolver;

        public InitGlass(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }


        public void Execute()
        {
            if (global::Glass.Mapper.Context.Default != null)
                return;//already initialized

            var resolver = DependencyResolver.CreateStandardResolver();
            //install the custom services
            resolver.Container.Install(new SitecoreInstaller());
            resolver.Container.Register(
                Component.For<IObjectConstructionTask>().ImplementedBy<SearchProxyWrapperTask>().LifestyleTransient()
            );


            //create a context
            var context = global::Glass.Mapper.Context.Create(resolver);
            context.Load(_resolver.ResolveAll<IConfigurationLoader>().ToArray());

            if (!global::Sitecore.Configuration.Settings.GetBoolSetting("Glass.CodeFirst", false)) return;

            using (new SecurityDisabler())
            {
                var dbs = global::Sitecore.Configuration.Factory.GetDatabases();
                foreach (var db in dbs)
                {
                    var provider = db.GetDataProviders().FirstOrDefault(x => x is GlassDataProvider) as GlassDataProvider;
                    if (provider != null)
                    {
                        provider.Initialise(db);
                    }
                }
            }


        }
    }

}
