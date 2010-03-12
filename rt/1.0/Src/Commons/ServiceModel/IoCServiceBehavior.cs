using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace BoC.ServiceModel
{
    public class IoCServiceBehavior : Attribute, IServiceBehavior
    {
        private ServiceHost serviceHost;

        #region IServiceBehavior Members

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                var cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider = new IoCInstanceProvider(serviceDescription.ServiceType);
                    }
                }
            }
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters) {}

        public void Validate(ServiceDescription serviceDescription,ServiceHostBase serviceHostBase) {}

        #endregion

        public void AddToHost(ServiceHost host)
        {
            // only add to host once
            if (serviceHost != null)
            {
                return;
            }
            host.Description.Behaviors.Add(this);
            serviceHost = host;
        }
    }
}