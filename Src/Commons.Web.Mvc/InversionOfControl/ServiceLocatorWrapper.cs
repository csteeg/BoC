using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.InversionOfControl
{
    public class ServiceLocatorWrapper : IServiceLocator
    {
        private readonly IDependencyResolver dependencyResolver;

        public ServiceLocatorWrapper(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public object GetService(Type serviceType)
        {
            var service = dependencyResolver.Resolve(serviceType);
            if (service == null)
                throw new ActivationException("Could not create an instance of type " + serviceType);

            return service;

        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return dependencyResolver.ResolveAll<TService>() ?? Enumerable.Empty<TService>();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (dependencyResolver.ResolveAll(serviceType) ?? Enumerable.Empty<object>()).Cast<object>();
        }

        public TService GetInstance<TService>()
        {
            var service = dependencyResolver.Resolve<TService>();
            if (service == null)
                throw new ActivationException("Could not create an instance of type " + typeof(TService));

            return service;
        }

        public TService GetInstance<TService>(string key)
        {
            var service = dependencyResolver.Resolve<TService>(key);
            if (service == null)
                throw new ActivationException("Could not create an instance of type " + typeof(TService));

            return service;
        }

        public object GetInstance(Type serviceType)
        {
            return dependencyResolver.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return dependencyResolver.Resolve(serviceType, key);
        }
    }
}
