using System.Security.Permissions;
using System.Web;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class MvcFieldTemplate<TEntity> : MvcFieldTemplate where TEntity : class
    {
        public new TEntity Entity {
            get { return (TEntity)base.Entity; }
        }
    }
}