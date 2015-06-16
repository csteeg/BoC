using System;
using System.Net.Mime;
using System.Web;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.DataContext;
using BoC.Web;
using BoC.Web.Events;
using Microsoft.Web.Infrastructure;

namespace BoC.Web
{
    public class EventTriggeringHttpModule : IHttpModule
	{
        private static bool startWasCalled = false;

		private static void PublishEvent<T, TEventArgs>(Func<TEventArgs> args) where T: BaseEvent<TEventArgs>, new()
		{
            if (!IoC.IsInitialized())
                return;
			var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<T>().Publish(args());
			}
		}

		private static void PublishEvent<T>() where T : BaseEvent<WebRequestEventArgs>, new()
		{
            if (!IoC.IsInitialized())
                return;
            var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<T>().Publish(new WebRequestEventArgs(new HttpContextWrapper(HttpContext.Current)));
			}

		}

		public void Init(HttpApplication context)
		{
			if (!startWasCalled)
			{
                PublishEvent<WebApplicationStartEvent, WebApplicationEventArgs>(() => new WebApplicationEventArgs(context));
			    startWasCalled = true;
			}
            
			context.BeginRequest += (sender, args) => PublishEvent<WebRequestBeginEvent>();
			context.EndRequest += (sender, args) => PublishEvent<WebRequestEndEvent>();
			context.PostAuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
			context.AuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
			context.AuthenticateRequest += (sender, args) => PublishEvent<WebAuthenticateEvent>();
			context.PostAuthenticateRequest += (sender, args) => PublishEvent<WebPostAuthenticateEvent>();
			context.Error += (sender, args) => PublishEvent<WebRequestErrorEvent>();
			context.PreRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPreHandlerExecute>();
			context.PostRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPostHandlerExecute>();
		}

		public void Dispose()
		{
		}
	}
}
