using System;
using System.Net.Mime;
using System.Web;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.UnitOfWork;
using BoC.Web.Events;

namespace BoC.Web
{
    public class EventTriggeringHttpModule : IHttpModule
    {
        private static string unitofworkkey = "BoC.Web.CommonHttpApplication.OuterUnitOfWork";

        private volatile static bool initialized = false;

        protected virtual void InitializeApplication()
        {
            if (!initialized)
            {
                initialized = true;
                InitializeBoC();
            }
        }

        protected virtual void InitializeBoC()
        {
            Initializer.Execute();
        }


        virtual protected void Session_Start()
        {
            PublishEvent<WebSessionBeginEvent>();
        }

        virtual protected void Session_End(object sender, EventArgs e)
        {
            PublishEvent<WebSessionEndEvent, SessionEndEventArgs>(() => new SessionEndEventArgs(new HttpSessionStateWrapper(Session)));
        }

        private void PublishEvent<T, TEventArgs>(Func<TEventArgs> args) where T: BaseEvent<TEventArgs>, new()
        {
            var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(args());
            }

        }

        private void PublishEvent<T>() where T : BaseEvent<WebRequestEventArgs>, new()
        {
            var eventAggregator = IoC.Resolver.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(new WebRequestEventArgs(new HttpContextWrapper(HttpContext.Current)));
            }

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) =>
            {
                ((HttpApplication)sender).Context.Items[unitofworkkey] = UnitOfWork.UnitOfWork.BeginUnitOfWork();
                PublishEvent<WebRequestBeginEvent>();
            };
            context.EndRequest += (sender, args) =>
            {
                PublishEvent<WebRequestEndEvent>();

                var unitOfWork = ((HttpApplication)sender).Context.Items[unitofworkkey] as IUnitOfWork;
                if (unitOfWork != null)
                {
                    unitOfWork.Dispose();
                }
                ((HttpApplication)sender).Context.Items.Remove(unitofworkkey);
            };
            context.PostAuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
            context.AuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
            context.AuthenticateRequest += (sender, args) => PublishEvent<WebAuthenticateEvent>();
            context.PostAuthenticateRequest += (sender, args) => PublishEvent<WebPostAuthenticateEvent>();
            context.Error += (sender, args) => PublishEvent<WebRequestErrorEvent>();
            context.PreRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPreHandlerExecute>();
            context.PostRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPostHandlerExecute>();

            if (!initialized)
            try
            {
                context.Application.Lock();
                lock (typeof(EventTriggeringHttpModule))
                {
                    InitializeApplication();
                }
            }
            finally
            {
                context.Application.UnLock();
            }

            PublishEvent<WebApplicationStartEvent, WebApplicationEventArgs>(() => new WebApplicationEventArgs(context));
        }

        public void Dispose()
        {
            PublishEvent<WebApplicationEndEvent, EventArgs>(() => new EventArgs());
        }
    }
}
