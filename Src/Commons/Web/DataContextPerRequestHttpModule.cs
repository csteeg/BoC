using System;
using System.Web;
using BoC.EventAggregator;
using BoC.Tasks;
using BoC.DataContext;
using BoC.Web.Events;

namespace BoC.Web
{
    //we have to use an httpmodule instead of subscribing to our own eventmanager, since this absolutely has to be started
    //immediately after the request starts.
    public class DataContextPerRequestHttpModule : IHttpModule
    {
        private const string DataContextkey = "BoC.DataContext.Web.OuterDataContext";
        public static bool Enabled = true;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
        }

        private void OnEndRequest(object sender, EventArgs eventArgs)
        {
            if (!Enabled)
            {
                return;
            }

            var DataContext = ((HttpApplication)sender).Context.Items[DataContextkey] as IDataContext;
            if (DataContext != null)
            {
                DataContext.Dispose();
            }
            ((HttpApplication)sender).Context.Items.Remove(DataContextkey);
        }

        private void OnBeginRequest(object sender, EventArgs eventArgs)
        {
            if (Enabled)
                ((HttpApplication)sender).Context.Items[DataContextkey] = DataContext.DataContext.BeginDataContext();
        }

    }
}
