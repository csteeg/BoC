using System;
using System.Web.Mvc;
using BoC.EventAggregator;
using BoC.Events;
using BoC.InversionOfControl;

namespace BoC.Web.Mvc.Attributes
{
    public class EventTriggerAttribute : ActionFilterAttribute
    {
        private static Type actionExecuted = typeof (ActionExecutedEvent<>);
        private static Type actionExecuting = typeof(ActionExecutingEvent<>);
        private static Type resultExecuting = typeof (ResultExecutingEvent<>);
        private static Type resultExecuted = typeof(ResultExecutedEvent<>);

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            PublishEvent<ActionExecutedEvent, ActionExecutedContext>(() => filterContext);
            PublishEvent(
                actionExecuted.MakeGenericType(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType),
                filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            PublishEvent<ActionExecutingEvent, ActionExecutingContext>(() => filterContext);
            PublishEvent(
                actionExecuting.MakeGenericType(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType),
                filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            PublishEvent<ResultExecutedEvent, ResultExecutedContext>(() => filterContext);
            PublishEvent(
                resultExecuted.MakeGenericType(filterContext.Controller.GetType()),
                filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            PublishEvent<ResultExecutingEvent, ResultExecutingContext>(() => filterContext);
            PublishEvent(
                resultExecuting.MakeGenericType(filterContext.Controller.GetType()),
                filterContext);
        }

        private void PublishEvent<T, TEventArgs>(Func<TEventArgs> args) where T : BaseEvent<TEventArgs>, new()
        {
            var eventAggregator = IoC.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<T>().Publish(args());
            }
        }

        private void PublishEvent(Type eventType, object args)
        {
            var eventAggregator = IoC.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent(eventType).Publish(args);
            }
        }

    }
}
