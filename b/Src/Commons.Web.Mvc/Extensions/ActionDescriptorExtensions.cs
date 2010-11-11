using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Extensions
{
    public static class ActionDescriptorExtensions
    {
        public static bool IsAction<TController>(this ActionDescriptor actionDescriptor, Expression<Action<TController>> action) where TController: IController
        {
            if (actionDescriptor is ReflectedActionDescriptor)
            {
                return typeof (TController).IsAssignableFrom(actionDescriptor.ControllerDescriptor.ControllerType) &&
                       ((ReflectedActionDescriptor) actionDescriptor).MethodInfo.ToString() ==
                       ((MethodCallExpression) action.Body).Method.ToString();
            }
            return (typeof (TController)).IsAssignableFrom(actionDescriptor.ControllerDescriptor.ControllerType) &&
                   ((MethodCallExpression) action.Body).Method.Name == actionDescriptor.ActionName;
        }
    }
}
