using System;
using System.Web.DomainServices;
using BoC.InversionOfControl;

namespace BoC.DomainServices
{
    public class IoCDomainServiceFactory : IDomainServiceFactory
    {
        public DomainService CreateDomainService(Type domainServiceType, DomainServiceContext context)
        {
            if (!typeof(DomainService).IsAssignableFrom(domainServiceType))
            {
                throw new ArgumentException(String.Format("Cannot create an instance of {0} since it does not inherit from DomainService", domainServiceType),  "domainServiceType");
            }
            var service = (DomainService) IoC.Resolve(domainServiceType);
            service.Initialize(context);
            return service;
        }

        public void ReleaseDomainService(DomainService domainService)
        {
            domainService.Dispose();
        }
    }
}
