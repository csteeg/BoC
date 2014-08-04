using System;
using System.Linq;
using BoC.Helpers;

namespace BoC.InversionOfControl
{
    public static class IoC
    {
        private static readonly object resolverLock = new object();

        public static void InitializeWith(IDependencyResolver resolver)
        {
            Check.Argument.IsNotNull(resolver, "resolver");
            lock (resolverLock)
            {
                if (IoC.Resolver != null)
                    throw new ApplicationException("IoC already initialized");
                
                IoC.Resolver = resolver;
                resolver.RegisterInstance<IDependencyResolver>(resolver);
                RunContainerInitializers(resolver);
            }
        }

        public static bool IsInitialized()
        {
            return Resolver != null;
        }

        private static IDependencyResolver resolver;
        public static IDependencyResolver Resolver
        {
            get { return resolver; }
            private set
            {
                lock (resolverLock)
                {
                    resolver = value;
                }
            }
        }

        public static void Reset()
        {
            lock (resolverLock)
            {
                Resolver = null;
            }
        }

        private static void RunContainerInitializers(IDependencyResolver dependencyResolver)
        {
            var appdomainHelpers = dependencyResolver.ResolveAll<IAppDomainHelper>();
            if (appdomainHelpers == null || ! appdomainHelpers.Any())
            {
                appdomainHelpers = new[] {AppDomainHelper.CreateDefault()};
            }

            var initTasks =
                appdomainHelpers.SelectMany(helper => helper.GetTypes(
                    t => t.IsClass && !t.IsAbstract && typeof (IContainerInitializer).IsAssignableFrom(t)));
            //register them:
            foreach (var taskType in initTasks)
            {
                dependencyResolver.RegisterType(typeof(IContainerInitializer), taskType);
            }
            //run them:
            var allTasks = dependencyResolver.ResolveAll<IContainerInitializer>()
                                             .Where( it=> it != null )
                                             .ToList();
            //first user's tasks:
            foreach (var task in allTasks.Where(t => !NamespaceStartsWith (t , "BoC.")))
            {
                task.Execute();
            }
            //now ours:
            foreach (var task in allTasks.Where(t => NamespaceStartsWith(t,"BoC.")))
            {
                task.Execute();
            }
        }

        private static bool NamespaceStartsWith(IContainerInitializer initializer, String startsWidth )
        {
            if (initializer == null) return false;

            var typeofInitializerNameSpace = initializer.GetType().Namespace;
            if (String.IsNullOrEmpty(typeofInitializerNameSpace)) return false;

            return typeofInitializerNameSpace.StartsWith(startsWidth);
        }
    }
}