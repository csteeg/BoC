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
                (t) => this.dependencyResolver.ResolveAll(t).Cast<object>());
        }

    }
}