using System.Web.Http;
using System.Web.Mvc;
using BoC.EventAggregator;
using BoC.Tasks;
using BoC.Web.Mvc.IoC;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Sitecore.Mvc.Initialize
{
    public class SetDefaultMvcDependencyResolver : IBootstrapperTask
    {
        public static bool Disabled = false;
        private readonly SitecoreSpecificResolver _dependencyResolver;

        public SetDefaultMvcDependencyResolver(IDependencyResolver dependencyResolver, IEventAggregator eventAggregator)
        {
            global::BoC.Web.Mvc.Init.SetDefaultMvcDependencyResolver.Disabled = true;
            _dependencyResolver = new SitecoreSpecificResolver(dependencyResolver, eventAggregator);
        }

        public void Execute()
        {
            if (!Disabled)
            {
                DependencyResolver.SetResolver(_dependencyResolver);
                GlobalConfiguration.Configuration.DependencyResolver = _dependencyResolver;
            }
        }
    }
}