using System;
using System.Web.Http.Dependencies;
using BoC.EventAggregator;
using BoC.Web.Mvc.IoC;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Sitecore.Mvc
{
    public class SitecoreSpecificResolver: BoCDependencyResolver
    {
        public SitecoreSpecificResolver(IDependencyResolver resolver, IEventAggregator eventAggregator) : base(resolver, eventAggregator)
        {
        }

        protected SitecoreSpecificResolver(IDependencyResolver resolver) : base(resolver)
        {
        }


      public override object GetService(Type serviceType)
      {
        //sitecore "injects" it's own dependencies in it's controllers (and others), so don't resolve sitecore classes, just construct them
        //now habitat also uses sitecore.* names for their dll's, let's skip those as much as possible
        if (!serviceType.Assembly.FullName.StartsWith("sitecore.feature.", StringComparison.InvariantCultureIgnoreCase)
            && !serviceType.Assembly.FullName.StartsWith("sitecore.foundation.", StringComparison.InvariantCultureIgnoreCase)
            && !serviceType.Assembly.FullName.StartsWith("sitecore.common.", StringComparison.InvariantCultureIgnoreCase)
            && serviceType.Assembly.FullName.StartsWith("sitecore.", StringComparison.InvariantCultureIgnoreCase))
        {
          return Activator.CreateInstance(serviceType);
        }
        return base.GetService(serviceType);
      }

      public override IDependencyScope BeginScope()
        {
            return new SitecoreSpecificResolver(_resolver.CreateChildResolver());
        }
    }
}
