using System.Web;
using System.Web.Routing;

namespace JqMvcSampleWeb
{
	public class GlobalApplication :HttpApplication
	{
        protected void Application_Start()
        {
            JqueryMvc.StartUp.RegisterDefaultRoutes(RouteTable.Routes);
            JqueryMvc.StartUp.SetupViewEngine();
        }
	}
}