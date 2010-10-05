using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.InversionOfControl;
using BoC.Persistence;
using BoC.Web.Mvc.Controllers;

namespace BoC.Web.Mvc
{
    public class AutoScaffoldControllerFactory : DefaultControllerFactory
    {
        private static Hashtable controllerTypeCache = new Hashtable();

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var result = base.GetControllerType(requestContext, controllerName);
            if (result != null)
            {
                return result;
            }

            if (controllerTypeCache.ContainsKey(controllerName.ToLower()))
            {
                return controllerTypeCache[controllerName.ToLower()] as Type;
            }
            var ass = AppDomain.CurrentDomain.GetAssemblies().Where(
                a => !a.FullName.StartsWith("System.") &&
                     !a.FullName.StartsWith("Microsoft."));

            Type entityType =
                ass.SelectMany(a => a.GetTypes()).Where(
                    t =>
                        typeof(IBaseEntity).IsAssignableFrom(t) &&
                        t.Name.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase)
                        ).FirstOrDefault();

            if (entityType == null)
            {
                controllerTypeCache[controllerName.ToLower()] = null;
                return null;
            }

            return (Type)(controllerTypeCache[controllerName.ToLower()] = typeof(DynamicScaffoldController<>).MakeGenericType(entityType));

        }
    }


       
    }

}