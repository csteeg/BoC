using System.Web.Http;
using System.Web.Http.Dispatcher;
using BoC.Tasks;
using BoC.Web.Mvc.IoC;

namespace BoC.Web.Mvc.Init
{
    //public class SetDefaultHttpControllerActivator : IBootstrapperTask
    //{
    //    public void Execute()
    //    {
    //        if (GlobalConfiguration.Configuration.Services.GetService(typeof(IHttpControllerActivator)) is DefaultHttpControllerActivator)
    //            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new IoCHttpControllerActivator());
    //    }

    //}
}