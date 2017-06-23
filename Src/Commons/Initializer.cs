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
        public static bool Executed { get; set; }
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

            Execute(CreateDependencyResolver(appDomainHelpers), appDomainHelpers);
        }

	    public static IDependencyResolver CreateDependencyResolver(IAppDomainHelper[] appDomainHelpers)
	    {
		    var depResolverTypeName = ConfigurationManager.AppSettings["BoC.IoC.ResolverTypeName"];
		    Type depresolverType = null;
		    if (depResolverTypeName != null)
		    {
			    depresolverType = Type.GetType(depResolverTypeName, false);
		    }
		    if (depresolverType == null && appDomainHelpers != null)
		    {
			    depresolverType = appDomainHelpers.SelectMany(a =>
							    a.GetTypes(t => typeof(IDependencyResolver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.IsPublic))
				    .FirstOrDefault();
		    }
		    return depresolverType == null ? null : Activator.CreateInstance(depresolverType) as IDependencyResolver;
	    }

	    public static void Execute(IDependencyResolver dependencyResolver, params IAppDomainHelper[] appDomainHelpers)
        {
            if (Executed)
                return;
            lock (initializer_lock)
            {
                if (Executed)
                    return;
                IoC.InitializeWith(dependencyResolver, appDomainHelpers);
                dependencyResolver.Resolve<Bootstrapper>().Run();
                Executed = true;
            }

        }
    }
}
