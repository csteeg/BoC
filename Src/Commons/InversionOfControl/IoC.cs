using System;
using System.Collections.Generic;
using System.Linq;
using BoC.Helpers;

namespace BoC.InversionOfControl
{
    public static class IoC
    {
        private static readonly object resolverLock = new object();

        public static void InitializeWith(IDependencyResolver resolver, params IAppDomainHelper[] appDomainHelpers)
        {
            Check.Argument.IsNotNull(resolver, "resolver");
            if (IoC.Resolver != null)
                throw new ApplicationException("IoC already initialized");
            lock (resolverLock)
            {
                if (IoC.Resolver != null)
                    throw new ApplicationException("IoC already initialized");
                
                IoC.Resolver = resolver;
                resolver.RegisterInstance<IDependencyResolver>(resolver);
                RunContainerInitializers(resolver, appDomainHelpers);
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

        private static void RunContainerInitializers(IDependencyResolver dependencyResolver, IAppDomainHelper[] appdomainHelpers)
        {
            if (appdomainHelpers == null || appdomainHelpers.Any() )
            {
                var helper = AppDomainHelper.CreateDefault();
                appdomainHelpers = new[] {helper};
            }

            if (!dependencyResolver.IsRegistered(typeof (IAppDomainHelper)))
            {
                foreach (var helper in appdomainHelpers)
                {
                    dependencyResolver.RegisterInstance<IAppDomainHelper>(helper);
                }
            }

            var allTasks =
                appdomainHelpers.SelectMany(helper => helper.GetTypes(
                    t => t.IsClass && !t.IsAbstract && typeof (IContainerInitializer).IsAssignableFrom(t)));
            //run them:
            var allTasksList = allTasks as IList<Type> ?? allTasks.ToList();
            
            var nonbocHelpers = allTasksList.Where(t => t.Namespace != null && !t.Namespace.StartsWith("BoC."));
            var bocHelpers = allTasksList.Where(t => t.Namespace != null && t.Namespace.StartsWith("BoC."));

            //first user's tasks:
            foreach (var type in nonbocHelpers)
            {
                var task = CreateTask(dependencyResolver, type, appdomainHelpers);
                task.Execute();
            }
            //now ours:
            foreach (var type in bocHelpers )
            {
                var task = CreateTask(dependencyResolver, type, appdomainHelpers);
                task.Execute();
            }
        }

        private static IContainerInitializer CreateTask(IDependencyResolver dependencyResolver, Type type, IAppDomainHelper[] appDomainHelpers)
        {
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters();
            if (!parameters.Any())
                return  Activator.CreateInstance(type) as IContainerInitializer;
            var parameterValues = new object[parameters.Length];
            for (var i=0;i<parameters.Length;i++)
            {
                if (typeof (IDependencyResolver).IsAssignableFrom(parameters[i].ParameterType))
                    parameterValues[i] = dependencyResolver;
                else if (parameters[i].ParameterType == typeof (IAppDomainHelper[]))
                    parameterValues[i] = appDomainHelpers;
                else
                {
                    parameterValues[i] = Activator.CreateInstance(parameters[i].ParameterType);
                }
            }
            return constructor.Invoke(parameterValues) as IContainerInitializer;
        }
    }
}