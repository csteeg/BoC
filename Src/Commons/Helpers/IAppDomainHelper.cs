using System;
using System.Collections.Generic;
using System.Reflection;

namespace BoC.Helpers
{
    public interface IAppDomainHelper
    {
        IEnumerable<Assembly> GetAssemblies();
        IEnumerable<Type> GetTypes(Func<Type, bool> where);
        void Refresh();
    }
}