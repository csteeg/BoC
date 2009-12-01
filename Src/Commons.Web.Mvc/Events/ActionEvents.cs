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
    
}
