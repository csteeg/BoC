using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using BoC.EventAggregator;
using BoC.Web.Events;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.IoC
{
    public class BoCDependencyResolver: System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IDependencyResolver _resolver;
        private readonly IEventAggregator _eventAggregator;
        private static readonly object HttpContextKey = new object();
        private SubscriptionToken _subscription;
        private readonly bool _isChildContainer;

        public BoCDependencyResolver(IDependencyResolver resolver, IEventAggregator eventAggregator)
        {
            _resolver = resolver;
            _eventAggregator = eventAggregator;
            _subscription = eventAggregator.GetEvent<WebRequestEndEvent>().Subscribe(DisposeOfChildContainer);
        }

        private BoCDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
            _isChildContainer = true;
        }

        public object GetService(Type serviceType)
        {
            if (!_isChildContainer && typeof(IController).IsAssignableFrom(serviceType))
            {
                return HttpContextContainer.GetService(serviceType);
            } 
            return _resolver.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var resolveAll = _resolver.ResolveAll(serviceType);
            return resolveAll != null ? resolveAll.Cast<object>() : Enumerable.Empty<object>();
        }

        public IDependencyScope BeginScope()
        {
            return new BoCDependencyResolver(_resolver.CreateChildResolver());
        }

        public void Dispose()
        {
            if (_subscription != null && _eventAggregator != null)
            {
                _eventAggregator.GetEvent<WebRequestEndEvent>().Unsubscribe(_subscription);
            }
            _subscription = null;
            //I think it's best not to dispose the outer IDependencyResolver here, we have Dispose(true) to force anyway
        }

        ~BoCDependencyResolver()
        {
            Dispose(false);
        }

        public void Dispose(bool disposing)
        {
            if (disposing && _resolver != null)
            {
                _resolver.Dispose();
            }
        }

        protected IDependencyScope HttpContextContainer
        {
            get
            {
                if (HttpContext.Current == null || _isChildContainer)
                    return this;

                var childContainer = HttpContext.Current.Items[HttpContextKey] as IDependencyScope;

                if (childContainer == null)
                {
                    HttpContext.Current.Items[HttpContextKey] = childContainer = BeginScope();
                }

                return childContainer;
            }
        }
        public static void DisposeOfChildContainer(WebRequestEventArgs args)
        {
            var childContainer = args.HttpContext.Items[HttpContextKey] as IDependencyScope;

            var resolver = childContainer as BoCDependencyResolver;
            if (resolver != null)
            {
                resolver.Dispose(true);
            }
            if (childContainer != null) childContainer.Dispose();
        }
    }
}
