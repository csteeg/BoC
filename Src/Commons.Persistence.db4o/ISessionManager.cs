using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;

namespace BoC.Persistence.db4o
{
    public interface ISessionManager
    {
        IObjectContainer Session { get; }
    }
}
