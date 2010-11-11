using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BoC.InversionOfControl;

namespace BoC.ServiceModel
{
    public class IoCServiceHost: ServiceHost
    {
        protected IoCServiceHost() {}

        public IoCServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses) {}
        public IoCServiceHost(object singletonInstance, params Uri[] baseAddresses) : base(singletonInstance, baseAddresses) {}

        protected override void OnOpening()
        {
            IoC.Resolver.Resolve<IoCServiceBehavior>().AddToHost(this);
            IoC.Resolver.Resolve<ServiceErrorLoggerBehavior>().AddToHost(this);
            base.OnOpening();
        }
    }

}
