using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoC.InversionOfControl
{
    public enum LifetimeScope
    {
        Transient,
        PerHttpRequest,
        PerThread,
        Unowned
    }
}
