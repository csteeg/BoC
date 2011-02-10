using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.Tasks;
using BoC.Web.Events;
using BoC.Web.Mvc.Attributes;
using BoC.Web.Mvc.Binders;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaultNameSpace : IBootstrapperTask
    {
        private readonly IEventAggregator eventAggregator;
        private SubscriptionToken subscriptionToken;

        public SetDefaultNameSpace(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Execute()
        {
            subscriptionToken = eventAggregator.GetEvent<WebApplicationStartEvent>().Subscribe(args =>
            {
                var rootNamespace = args.ApplicationInstance.GetType().BaseType.Namespace;
                ControllerBuilder.Current.DefaultNamespaces.Add(rootNamespace + ".*");

                eventAggregator.GetEvent<WebApplicationStartEvent>().Unsubscribe(subscriptionToken);
            });
        }

    }
}