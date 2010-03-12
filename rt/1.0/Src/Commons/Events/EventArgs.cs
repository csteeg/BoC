using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoC.Events
{
    public class EventArgs<TForType>
    {
        public EventArgs(TForType item)
        {
            Item = item;
        }

        public TForType Item { get; private set; }
    }
}
