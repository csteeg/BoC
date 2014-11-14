using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace BoC.Web.Mvc.IoC
{
    public class IoCHttpControllerActivator: IHttpControllerActivator
    {
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return InversionOfControl.IoC.Resolver.Resolve(controllerType) as IHttpController;
        }
    }

}
