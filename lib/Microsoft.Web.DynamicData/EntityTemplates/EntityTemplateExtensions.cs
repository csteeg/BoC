using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;

namespace Microsoft.Web.DynamicData {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class EntityTemplateExtensions {
        static IEntityTemplateFactory _entityTemplateFactory;

        public static IEntityTemplateFactory GetEntityTemplateFactory(this MetaModel model) {
            if (_entityTemplateFactory == null) {
                SetEntityTemplateFactory(model, new EntityTemplateFactory());
            }

            return _entityTemplateFactory;
        }

        public static void SetEntityTemplateFactory(this MetaModel model, IEntityTemplateFactory factory) {
            _entityTemplateFactory = factory;
            _entityTemplateFactory.Initialize(model);
        }
    }
}