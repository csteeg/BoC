using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Tasks;

namespace BoC
{
    public static class Initializer
    {
        private static object initializer_lock = new object();
        public static bool Executed { get; private set; }
        public static void Execute()
        {
            if (Executed)
                return;
            Execute(AppDomainHelper.CreateDefault());
        }

        public static void Execute(params IAppDomainHelper[] appDomainHelpers)
        {
            if (Executed)
                return;

            var depResolverTypeName = ConfigurationManager.AppSettings["BoC.IoC.ResolverTypeName"];
            Type depresolverType = null;
            if (depResolverTypeName != null)
            {
                depresolverType = Type.GetType(depResolverTypeName, false);
            }
            if (depresolverType == null && appDomainHelpers != null)
            {
                depresolverType = appDomainHelpers.SelectMany(
                    a => a.GetTypes(t => typeof(IDependencyResolver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.IsPublic)).FirstOrDefault();
            }
            if (depresolverType != null)
            {
                Execute(Activator.CreateInstance(depresolverType) as IDependencyResolver, appDomainHelpers);
            }
        }

        public static void Execute(IDependencyResolver dependencyResolver, params IAppDomainHelper[] appDomainHelpers)
        {
            if (Executed)
                return;

            lock (initializer_lock)
            {
                if (appDomainHelpers != null)
                {
                    foreach (var appDomainHelper in appDomainHelpers)
                    {
                        dependencyResolver.RegisterInstance<IAppDomainHelper>(appDomainHelper);
                    }
                }

                IoC.InitializeWith(dependencyResolver);
                var bootstrapper = dependencyResolver.Resolve<Bootstrapper>();
                if (bootstrapper != null)
                    bootstrapper.Run();
                Executed = true;
            }

        }
    }
}
