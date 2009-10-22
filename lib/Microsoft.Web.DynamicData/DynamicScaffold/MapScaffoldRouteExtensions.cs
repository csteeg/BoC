using System;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class MapScaffoldRouteExtensions {
        static bool _addedFactory = false;

        public const string DATATOKENS_MetaModel = "DynamicDataMetaModel";

        public const string ROUTEDATA_Controller = "controller";
        public const string ROUTEDATA_Table = "table";

        public static void MapScaffoldRoute(this RouteCollection routes, string name, string url, MetaModel metaModel, object defaults) {
            MapScaffoldRoute(routes, name, url, metaModel, defaults, null /* constraints */);
        }

        public static void MapScaffoldRoute(this RouteCollection routes, string name, string url, MetaModel metaModel, object defaults, object constraints) {
            if (metaModel == null)
                throw new ArgumentNullException("metaModel");

            if (!_addedFactory) {
                _addedFactory = true;
                ControllerBuilder.Current.SetControllerFactory(new AutoScaffoldControllerFactory(ControllerBuilder.Current.GetControllerFactory()));
            }

            var dataTokens = new RouteValueDictionary();
            dataTokens[DATATOKENS_MetaModel] = metaModel;

            var defaultsDictionary = new RouteValueDictionary(defaults);
            var constraintsDictionary = new RouteValueDictionary(constraints);
            var route = new Route(url, defaultsDictionary, constraintsDictionary, dataTokens, new ScaffoldMvcRouteHandler());
            routes.Add(name, route);
        }

        class ScaffoldMvcRouteHandler : MvcRouteHandler {
            protected override IHttpHandler GetHttpHandler(RequestContext context) {
                // We want our routes expressed in terms of tables, but we still want to leverage the
                // MVC auto-discovery of controllers, so we force controller name == table name.
                if (context.RouteData.DataTokens.ContainsKey(DATATOKENS_MetaModel)) {
                    var table = context.RouteData.GetRequiredString(ROUTEDATA_Table);
                    context.RouteData.Values[ROUTEDATA_Controller] = table;
                }

                return base.GetHttpHandler(context);
            }
        }

        class AutoScaffoldControllerFactory : IControllerFactory {
            IControllerFactory _innerFactory;

            public AutoScaffoldControllerFactory(IControllerFactory innerFactory) {
                _innerFactory = innerFactory;
            }

            public IController CreateController(RequestContext context, string controllerName) {
                IController result = null;
                MetaModel metaModel = context.RouteData.DataTokens[DATATOKENS_MetaModel] as MetaModel;

                try {
                    result = _innerFactory.CreateController(context, controllerName);
                }
                catch (Exception) {
                    if (metaModel != null) {
                        MetaTable table;

                        if (metaModel.TryGetTable(context.RouteData.GetRequiredString(ROUTEDATA_Table), out table) && table.Scaffold) {
                            Type controllerType = typeof(DynamicScaffoldController<>).MakeGenericType(table.EntityType);
                            result = (IController)Activator.CreateInstance(controllerType);
                        }
                    }

                    if (result == null)
                        throw;
                }

                Controller controller = result as Controller;
                if (controller != null && metaModel != null)
                    controller.ViewData.SetMetaModel(metaModel);

                return result;
            }

            public void ReleaseController(IController controller) {
                _innerFactory.ReleaseController(controller);
            }
        }
    }
}