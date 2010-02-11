using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BoC.EventAggregator;

namespace BoC.Events
{
    public class ActionExecutedEvent : BaseEvent<ActionExecutedContext>{}
    public class ActionExecutingEvent : BaseEvent<ActionExecutingContext> { }
    public class ResultExecutingEvent : BaseEvent<ResultExecutingContext> { }
    public class ResultExecutedEvent : BaseEvent<ResultExecutedContext> { }

    public class ActionExecutedEvent<TController> : ActionExecutedEvent { }
    public class ActionExecutingEvent<TController> : ActionExecutingEvent { }
    public class ResultExecutingEvent<TController> : ResultExecutingEvent { }
    public class ResultExecutedEvent<TController> : ResultExecutedEvent { }
}
