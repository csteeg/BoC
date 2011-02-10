using System.Web.Mvc;
using BoC.Tasks;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaultViewEngine : IBootstrapperTask
    {
        public void Execute()
        {
            //replace webformviewengine with our version (that searches only .ascx for partials and only .aspx for fullviews)
            for (var i = ViewEngines.Engines.Count - 1; i >= 0; i--)
            {
                if (ViewEngines.Engines[i] is WebFormViewEngine)
                {
                    ViewEngines.Engines[i] = new DefaultViewViewEngine();
                    break; //only do the first one
                }
            }
        }

   }
}