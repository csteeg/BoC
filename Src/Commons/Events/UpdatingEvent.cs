using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.EventAggregator;

namespace BoC.Events
{
    public class UpdatingEvent<TForType> : BaseEvent<EventArgs<TForType>>
    {
    }
}
