using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Syntax;
using Ninject.Web.Common;

namespace BoC.InversionOfControl.Ninject
{
    public static class Extensions
    {
        public static IBindingNamedWithOrOnSyntax<T> SetLifeStyle<T>(this IBindingInSyntax<T> registration, LifetimeScope lifeTimeKey)
        {
            switch (lifeTimeKey)
            {
                case LifetimeScope.Unowned:
                    return registration.InTransientScope();
                case LifetimeScope.PerHttpRequest:
                    return registration.InRequestScope();
                case LifetimeScope.PerThread:
                    return registration.InThreadScope();
                default:
                    return registration.InTransientScope();
            }
        }
    }
}
