using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.EventAggregator;

namespace BoC.Events
{
    public class UpdatedEvent<TForType> : BaseEvent<EventArgs<TForType>>
    {
    }
}
