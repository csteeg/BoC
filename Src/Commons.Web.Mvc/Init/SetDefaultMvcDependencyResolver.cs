using System.Collections;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using BoC.EventAggregator;
using BoC.Tasks;
using BoC.Web.Mvc.IoC;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaultMvcDependencyResolver : IBootstrapperTask
    {
        public static bool Disabled = false;
        private readonly BoCDependencyResolver _dependencyResolver;

        public SetDefaultMvcDependencyResolver(IDependencyResolver dependencyResolver, IEventAggregator eventAggregator)
        {
            _dependencyResolver = new BoCDependencyResolver(dependencyResolver, eventAggregator);
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