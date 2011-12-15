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
    public class SetDefaultModelBinders : IBootstrapperTask
    {
        private readonly IDependencyResolver dependencyResolver;
        public SetDefaultModelBinders(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            ModelBinders.Binders.DefaultBinder = new CommonModelBinder(dependencyResolver);
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
			ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
			ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

        }

    }
}