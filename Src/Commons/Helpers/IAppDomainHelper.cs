using System;
using System.Collections.Generic;
using System.Reflection;

namespace BoC.Helpers
{
    public interface IAppDomainHelper
    {
        IEnumerable<Assembly> GetAssemblies();
        IEnumerable<Type> GetTypes(Func<Type, bool> where);
        ICollection<Func<Type, bool>> TypeFilters { get; }
        ICollection<Func<Assembly, bool>> AssemblyFilters { get; }
    }
}