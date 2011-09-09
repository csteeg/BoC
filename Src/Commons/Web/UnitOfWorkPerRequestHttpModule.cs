using System;
using System.Web;
using BoC.EventAggregator;
using BoC.Tasks;
using BoC.UnitOfWork;
using BoC.Web.Events;

namespace BoC.Web
{
    //we have to use an httpmodule instead of subscribing to our own eventmanager, since this absolutely has to be started
    //immediately after the request starts.
    public class UnitOfWorkPerRequestHttpModule : IHttpModule
    {
        private const string unitofworkkey = "BoC.UnitOfWork.Web.OuterUnitOfWork";

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
            var unitOfWork = ((HttpApplication)sender).Context.Items[unitofworkkey] as IUnitOfWork;
            if (unitOfWork != null)
            {
                unitOfWork.Dispose();
            }
            ((HttpApplication)sender).Context.Items.Remove(unitofworkkey);
        }

        private void OnBeginRequest(object sender, EventArgs eventArgs)
        {
            ((HttpApplication)sender).Context.Items[unitofworkkey] = UnitOfWork.UnitOfWork.BeginUnitOfWork();
        }

    }
}
