using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class ViewDataDictionaryExtensions {
        const string VIEWDATA_DynamicData = "__DynamicData__";
        const string VIEWDATA_MetaModel = "__MetaModel__";
        const string VIEWDATA_StatusMessage = "__StatusMessage__";

        public static DynamicDataHelper DynamicData(this ViewDataDictionary viewData) {
            return (DynamicDataHelper)viewData[VIEWDATA_DynamicData];
        }

        public static DynamicDataHelper<TEntity> DynamicData<TEntity>(this ViewDataDictionary viewData) where TEntity : class, new() {
            return (DynamicDataHelper<TEntity>)viewData[VIEWDATA_DynamicData];
        }

        public static MetaModel MetaModel(this ViewDataDictionary viewData) {
            if (viewData.ContainsKey(VIEWDATA_MetaModel))
                return (MetaModel)viewData[VIEWDATA_MetaModel];
            return System.Web.DynamicData.MetaModel.Default;
        }

        public static string StatusMessage(this ViewDataDictionary viewData) {
            return (string)viewData[VIEWDATA_StatusMessage];
        }

        public static void SetDynamicData(this ViewDataDictionary viewData, DynamicDataHelper dynamicData) {
            viewData[VIEWDATA_DynamicData] = dynamicData;
        }

        public static void SetMetaModel(this ViewDataDictionary viewData, MetaModel metaModel) {
            viewData[VIEWDATA_MetaModel] = metaModel;
        }

        public static void SetStatusMessage(this ViewDataDictionary viewData, string message) {
            viewData[VIEWDATA_StatusMessage] = message;
        }
    }
}
