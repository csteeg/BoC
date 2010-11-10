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
        public static void Execute()
        {
            Execute(AppDomainHelper.CreateDefault());
        }

        public static void Execute(params IAppDomainHelper[] appDomainHelpers)
        {
            var depResolverTypeName = ConfigurationManager.AppSettings["BoC.IoC.ResolverTypeName"];
            Type depresolverType = null;
            if (depResolverTypeName != null)
            {
                depresolverType = Type.GetType(depResolverTypeName, false);
            }
            if (depresolverType == null && appDomainHelpers != null)
            {
                depresolverType = appDomainHelpers.SelectMany(
                    a => a.GetTypes(t => typeof (IDependencyResolver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract &&t.IsPublic)).FirstOrDefault();
            }
            if (depresolverType != null)
            {
                Execute(Activator.CreateInstance(depresolverType) as IDependencyResolver, appDomainHelpers);
            }
        }

        public static void Execute(IDependencyResolver dependencyResolver, params IAppDomainHelper[] appDomainHelpers)
        {
            if (appDomainHelpers != null)
            {
                foreach (var appDomainHelper in appDomainHelpers)
                {
                    dependencyResolver.RegisterInstance<IAppDomainHelper>(appDomainHelper);
                }
            }

            IoC.InitializeWith(dependencyResolver);
            dependencyResolver.Resolve<Bootstrapper>().Run();
        }
    }
}
