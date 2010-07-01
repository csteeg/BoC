using System;
using System.Web.Mvc;
using BoC.InversionOfControl;

namespace BoC.Web.Mvc
{
    public class IoCControllerFactory : DefaultControllerFactory
    {
        private readonly IDependencyResolver dependencyResolver;

        protected IoCControllerFactory(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return (controllerType == null) ? base.GetControllerInstance(requestContext, controllerType) : dependencyResolver.Resolve(controllerType) as IController;
        }
    }
}