using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.EventAggregator;

namespace BoC.Events
{
    public class DeletingEvent<TForType> : BaseEvent<EventArgs<TForType>>
    {
    }
}
