using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.WebPages;
using BoC.Security.Model;
using BoC.Web;
using Norm.Configuration;
using ToDoList;

namespace ToDoList
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : CommonHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected override void InitializeApplication()
        {
			base.InitializeApplication();
            
            System.Web.WebPages.ApplicationPart.Register(new ApplicationPart(typeof(MvcApplication).Assembly, "~/ToDoList"));

            MongoConfiguration.Initialize(config =>
                            config.For<User>(c => c.ForProperty(u => u.Identity).Ignore()));
        }
    }
}