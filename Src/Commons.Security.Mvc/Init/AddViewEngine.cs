using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using BoC.Tasks;
using BoC.Web.Mvc;
using RazorGenerator.Mvc;

namespace BoC.Security.Mvc.Init
{
    public class AddViewEngine : IBootstrapperTask
    {
        public void Execute()
        {
            var engine = new CompositePrecompiledMvcEngine(PrecompiledViewAssembly.OfType<AddViewEngine>());
            ViewEngines.Engines.Add(engine);
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }

   }
}