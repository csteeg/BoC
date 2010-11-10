using System;
using System.Web;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.UnitOfWork;
using BoC.Web.Events;

namespace BoC.Web
{
    public abstract class CommonHttpApplication : HttpApplication
    {
        private static string unitofworkkey = "BoC.Web.CommonHttpApplication.OuterUnitOfWork";

        private volatile static bool initialized = false;

        virtual protected void Application_Start()
        {
            try
            {
                Application.Lock();
                lock (typeof(CommonHttpApplication))
                {
                    InitializeApplication();
                }
            }
            finally
            {
                Application.UnLock();
            }

            PublishEvent<WebApplicationStartEvent, WebApplicationEventArgs>(() => new WebApplicationEventArgs(this));
        }

        virtual protected void Application_End()
        {
            PublishEvent<WebApplicationEndEvent, WebApplicationEventArgs>(() => new WebApplicationEventArgs(this));
        }

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

        public override void Init()
        {
            base.Init();
            AttachEvents();
        }

        protected virtual void AttachEvents()
        {
            this.BeginRequest += (sender, args) =>
                                     {
                                        if (!initialized)
                                            Application_Start();
                                        
                                        this.Context.Items[unitofworkkey] = UnitOfWork.UnitOfWork.BeginUnitOfWork();
                                        PublishEvent<WebRequestBeginEvent>();
                                     };
            this.EndRequest += (sender, args) =>
                                   {
                                        PublishEvent<WebRequestEndEvent>();

                                        var unitOfWork = this.Context.Items[unitofworkkey] as IUnitOfWork;
                                        if (unitOfWork != null)
                                        {
                                            unitOfWork.Dispose();
                                        }
                                        this.Context.Items.Remove(unitofworkkey);
                                   };
            this.PostAuthorizeRequest += (sender,args) => PublishEvent<WebPostAuthorizeEvent>();
            this.AuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
            this.AuthenticateRequest += (sender, args) => PublishEvent<WebAuthenticateEvent>();
            this.PostAuthenticateRequest += (sender, args) => PublishEvent<WebPostAuthenticateEvent>();
            this.Error += (sender,args) => PublishEvent<WebRequestErrorEvent>();
            this.PreRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPreHandlerExecute>();
            this.PostRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPostHandlerExecute>();
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
    }
}
