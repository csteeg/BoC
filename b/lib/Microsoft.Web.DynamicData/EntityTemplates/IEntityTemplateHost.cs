using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI.WebControls;

namespace Microsoft.Web.DynamicData {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public interface IEntityTemplateHost {
        DataBoundControlMode Mode { get; }
        MetaTable Table { get; }
    }
}