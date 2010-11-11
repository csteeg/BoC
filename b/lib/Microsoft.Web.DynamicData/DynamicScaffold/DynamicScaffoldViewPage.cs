using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DynamicScaffoldViewPage : ViewPage {
        DynamicDataHelper _dynamicDataHelper;

        protected DynamicDataHelper DynamicData {
            get {
                if (_dynamicDataHelper == null) {
                    _dynamicDataHelper = ViewData.DynamicData();
                }
                return _dynamicDataHelper;
            }
        }
    }
}