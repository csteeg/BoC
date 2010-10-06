using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.InversionOfControl
{
    public class DependencyResolverWrapper : System.Web.Mvc.IDependencyResolver
    {
        private readonly IDependencyResolver dependencyResolver;

        public DependencyResolverWrapper(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public object GetService(Type serviceType)
        {
            return dependencyResolver.Resolve(serviceType);

        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return dependencyResolver.ResolveAll(serviceType).OfType<object>() ?? Enumerable.Empty<object>();
        }
    }
}
