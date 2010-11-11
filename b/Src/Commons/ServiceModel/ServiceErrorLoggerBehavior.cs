using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using BoC.InversionOfControl;
using BoC.Logging;

namespace BoC.ServiceModel
{
    public class ServiceErrorLoggerBehavior : Attribute, IServiceBehavior
    {
        private ServiceHost serviceHost;

        #region IServiceBehavior Members

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var errorHandler = IoC.Resolver.Resolve<ServiceErrorLogger>();
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    channelDispatcher.ErrorHandlers.Add(errorHandler);
                }
            }
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters) { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

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
