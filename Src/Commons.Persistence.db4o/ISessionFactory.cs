using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;

namespace Commons.Persistence.db4o
{
    public interface ISessionFactory : IDisposable
    {
        IObjectContainer CreateSession();
    }
}
