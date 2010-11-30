using System.Web;
using System.Web.Mvc;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.Security.Model;
using BoC.Web;
using BoC.Web.Mvc.PrecompiledViews;
using Norm.Configuration;

namespace ToDoList
{

	public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

		public override void Init()
		{
			MongoConfiguration.Initialize(config =>
                            config.For<User>(c => c.ForProperty(u => u.Identity).Ignore()));

			base.Init();
        }

    }

}