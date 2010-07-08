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
                    throw new ArgumentException("IoC already initialized");
                
                IoC.Resolver = resolver;

                RunContainerInitailizers(resolver);
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
            set
            {
                lock (resolverLock)
                {
                    resolver = value;
                }
            }
        }

        private static void RunContainerInitailizers(IDependencyResolver dependencyResolver)
        {
            var appdomainHelpers = dependencyResolver.ResolveAll<IAppDomainHelper>();
            if (appdomainHelpers == null || appdomainHelpers.Count() == 0)
            {
                appdomainHelpers = new[] {AppDomainHelper.CreateDefault()};
            }

            var initTasks =
                appdomainHelpers.SelectMany(helper => helper.GetTypes(
                    t => t.IsClass && !t.IsAbstract && typeof (IContainerInitializer).IsAssignableFrom(t)));

            foreach (var t in initTasks.Where(t => !t.Namespace.StartsWith("BoC."))) //first user's tasks
            {
                RunContainerInitailizer(t, dependencyResolver);
            }
            foreach (var t in initTasks.Where(t => t.Namespace.StartsWith("BoC."))) //now ours
            {
                RunContainerInitailizer(t, dependencyResolver);
            }
        }

        private static void RunContainerInitailizer(Type type, IDependencyResolver dependencyResolver)
        {
            IContainerInitializer initializer = null;
            try
            {
                var constructor = type.GetConstructor(new[] {typeof (IDependencyResolver)});
                if (constructor != null)
                {
                    initializer = Activator.CreateInstance(type,new object[]{ dependencyResolver}) as IContainerInitializer;
                }
            }
            catch {}

            if (initializer == null)
                initializer = Activator.CreateInstance(type) as IContainerInitializer;
            
            if (initializer != null)
                initializer.Execute();
        }
    }
}