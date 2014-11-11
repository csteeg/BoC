using System.Collections;
using System.Linq;
using System.Web.Mvc;
using BoC.Tasks;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaultMvcDependencyResolver : IBootstrapperTask
    {
        private readonly IDependencyResolver dependencyResolver;

        public SetDefaultMvcDependencyResolver(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            DependencyResolver.SetResolver(
                this.dependencyResolver.Resolve,
                (t) =>
                {
                    IEnumerable resolveAll = this.dependencyResolver.ResolveAll(t);
                    return resolveAll != null ? resolveAll.Cast<object>() : Enumerable.Empty<object>();
                });
        }

    }
}