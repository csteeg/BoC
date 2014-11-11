using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using BoC.InversionOfControl;
using BoC.Tasks;

namespace BoC.Helpers
{
    public class BuildManagerAppDomainHelper: IAppDomainHelper
    {
        ICollection<Func<Type, bool>> typeFilters = new List<Func<Type, bool>>(){ type => true };
        ICollection<Func<Assembly, bool>> assemblyFilters = new List<Func<Assembly, bool>>() { a => true };
        public ICollection<Func<Type, bool>> TypeFilters
        {
            get { return typeFilters; }
        }

        public ICollection<Func<Assembly, bool>> AssemblyFilters
        {
            get { return assemblyFilters; }
        }

        public IEnumerable<Type> GetTypes(Func<Type, bool> where)
        {
            return GetAssemblies().SelectMany(
                a =>
                    {
                        try
                        {
                            return a.GetTypes().Where(t => where(t) && TypeFilters.All(func => func(t)));
                        }
                        catch (Exception exception)
                        {
                            var msg = string.Format("Loading assembly {0} failed: \n{1}", a.FullName, exception);
                            Trace.TraceWarning(msg);
                            Debugger.Log((int)TraceLevel.Warning, "BuildManagerAppDomainHelper", msg);
                            return new Type[0];
                        }
                    });
        }

        public void Refresh()
        {
            
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return BuildManager.GetReferencedAssemblies().Cast<Assembly>().Where(a => AssemblyFilters.All(func => func(a))); ;
        }
    }
}