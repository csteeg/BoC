using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            if (appdomainHelpers == null || appdomainHelpers.Count() == 0)
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
            var allTasks = dependencyResolver.ResolveAll<IContainerInitializer>();
            if (allTasks != null)
            {
                //first user's tasks:
                foreach (var task in allTasks.Where(t => !t.GetType().Namespace.StartsWith("BoC.")))
                {
                    task.Execute();
                }
                //now ours:
                foreach (var task in allTasks.Where(t => t.GetType().Namespace.StartsWith("BoC.")))
                {
                    task.Execute();
                }
            }
        }
    }
}