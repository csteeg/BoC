using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.Tasks;
using BoC.Web.Events;

namespace BoC.Web
{
    public abstract class CommonHttpApplication : HttpApplication
    {
        private static bool initialized = false;

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
        }

        protected virtual void InitializeApplication()
        {
            if (!initialized)
            {
                initialized = true;
                Bootstrapper.RegisterAllTasksAndRunThem(type => true);
            }
        }

        public override void Init()
        {
            base.Init();
            AttachEvents();
        }

        protected virtual void AttachEvents()
        {
            this.BeginRequest += (sender, args) => PublishEvent<WebRequestBeginEvent>();
            this.EndRequest += (sender, args) => PublishEvent<WebRequestEndEvent>();
            this.PostAuthorizeRequest += (sender,args) => PublishEvent<WebPostAuthorizeEvent>();
            this.AuthorizeRequest += (sender, args) => PublishEvent<WebPostAuthorizeEvent>();
            this.AuthenticateRequest += (sender, args) => PublishEvent<WebAuthenticateEvent>();
            this.PostAuthenticateRequest += (sender, args) => PublishEvent<WebPostAuthenticateEvent>();
            this.Error += (sender,args) => PublishEvent<WebRequestErrorEvent>();
            this.PreRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPreHandlerExecute>();
            this.PostRequestHandlerExecute += (sender, args) => PublishEvent<WebRequestPostHandlerExecute>();
        }

        virtual protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!initialized)
                Application_Start();
        }

        virtual protected void Session_Start()
        {
            PublishEvent<WebSessionBeginEvent>();
        }

        virtual protected void Session_End(object sender, EventArgs e)
        {
            PublishEvent<WebSessionEndEvent, SessionEndEventArgs>(() => new SessionEndEventArgs(new HttpSessionStateWrapper(Session)));
        }

        virtual protected void Application_PostAuthorizeRequest()
        {
            
        }

        private void PublishEvent<T, TEventArgs>(Func<TEventArgs> args) where T: BaseEvent<TEventArgs>, new()
        {
            var eventAggregator = IoC.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(args());
            }

        }

        private void PublishEvent<T>() where T : BaseEvent<WebRequestEventArgs>, new()
        {
            var eventAggregator = IoC.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(new WebRequestEventArgs(new HttpContextWrapper(HttpContext.Current)));
            }

        }
    }
}
