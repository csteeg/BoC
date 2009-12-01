using System;
using System.Web.Mvc;
using BoC.EventAggregator;
using BoC.Events;
using BoC.InversionOfControl;

namespace BoC.Web.Mvc.Attributes
{
    public class EventTriggerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            PublishEvent<ActionExecutedEvent, ActionExecutedContext>(() => filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            PublishEvent<ActionExecutingEvent, ActionExecutingContext>(() => filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            PublishEvent<ResultExecutedEvent, ResultExecutedContext>(() => filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            PublishEvent<ResultExecutingEvent, ResultExecutingContext>(() => filterContext);
        }

        private void PublishEvent<T, TEventArgs>(Func<TEventArgs> args) where T : BaseEvent<TEventArgs>, new()
        {
            var eventAggregator = IoC.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(args());
            }

        }


    }
}
