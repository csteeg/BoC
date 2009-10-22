using System.Security.Permissions;
using System.Web;

namespace Microsoft.Web.DynamicData {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public interface IEntityTemplate {
        void SetHost(IEntityTemplateHost host);
    }
}
