using System;
using System.Web.Mvc;
using BoC.InversionOfControl;

namespace BoC.Web.Mvc
{
    public class IoCControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return (controllerType == null) ? base.GetControllerInstance(requestContext, controllerType) : IoC.Resolve(controllerType) as IController;
        }
    }
}