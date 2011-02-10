using System.Web.Mvc;
using BoC.Tasks;
using BoC.Web.Mvc.Attributes;

namespace BoC.Web.Mvc.Init
{
    public class RegisterGlobalFilters : IBootstrapperTask
    {
        public void Execute()
        {
            Register(GlobalFilters.Filters);
        }

        internal void Register(GlobalFilterCollection filters)
        {
            filters.Add(new AjaxControllerAttribute());
            filters.Add(new EventTriggerAttribute());
            filters.Add(new HandleErrorAttribute());
        }

    }
}