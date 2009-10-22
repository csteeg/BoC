using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.DomainServices;
using System.Web.DynamicData.ModelProviders;

namespace BoC.DomainServices
{
    /// <summary>
    /// This class fixes 2 minor issues in the domainmodelprovider
    /// </summary>
    public class FixDomainModelProvider: DomainModelProvider
    {
        
        public FixDomainModelProvider(Type domainServiceType) : base(domainServiceType)
        {
            //TODO: remove this ugly stuff once domaincolumnprovider does it itself.
            //if MS is not going to fix it (sure they will) we'll have to create our own tableprovider & columnprovider
            PropertyInfo info = typeof(ColumnProvider).GetProperty("EntityTypeProperty");
            foreach (var table in this.Tables)
            {
                foreach (var column in table.Columns)
                {
                    info.SetValue(column, table.EntityType.GetProperty(column.Name, column.ColumnType), null);
                }
            }
        }

        public override object CreateContext()
        {
            var context = new DomainServiceContext(new AspNetServiceProvider(), DomainOperationType.Query);
            return DomainService.Factory.CreateDomainService(ContextType, context);
        }
    }


    //damn those ms internals
    internal class AspNetServiceProvider : IServiceProvider
    {
        private IPrincipal _user;

        public AspNetServiceProvider(): this(HttpContext.Current.User)
        {
        }

        public AspNetServiceProvider(IPrincipal user)
        {
            this._user = user;
        }

        public AspNetServiceProvider(HttpContextBase context)
        {
            this._user = context.User;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IPrincipal))
            {
                return this._user;
            }
            return null;
        }
    }

}
