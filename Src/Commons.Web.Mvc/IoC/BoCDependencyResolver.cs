using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.IoC
{
    public class BoCDependencyResolver: System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        private IDependencyResolver _resolver;

        public BoCDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            return _resolver.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var resolveAll = _resolver.ResolveAll(serviceType);
            return resolveAll != null ? resolveAll.Cast<object>() : Enumerable.Empty<object>();
        }

        public IDependencyScope BeginScope()
        {
            return new BoCDependencyResolver(_resolver.BeginScope());
        }

        public void Dispose()
        {
            _resolver = null;
        }
    }
}
