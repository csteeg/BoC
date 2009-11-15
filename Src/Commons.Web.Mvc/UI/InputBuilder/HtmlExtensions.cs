using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using MvcContrib.UI.InputBuilder;
using MvcContrib.UI.InputBuilder.Views;
using MvcContrib.UI.InputBuilder.InputSpecification;

namespace BoC.Web.Mvc.UI.InputBuilder
{
    public static class HtmlExtensions
    {
        static private Type inputType = typeof (Input<>);
        static private Type htmlHelperType = typeof(HtmlHelper<>);

        public static IInputSpecification<TypeViewModel> InputForm(this HtmlHelper htmlHelper, string controller, string action)
        {
            if (htmlHelper == null || htmlHelper.ViewData == null || htmlHelper.ViewData.Model == null)
                return null;

            var modelType = htmlHelper.ViewData.Model.GetType();
            var newHtmlHelper =
                Activator.CreateInstance(htmlHelperType.MakeGenericType(modelType), htmlHelper.ViewContext, htmlHelper.ViewDataContainer, htmlHelper.RouteCollection);
            
            var inp =
                Activator.CreateInstance(inputType.MakeGenericType(modelType), newHtmlHelper);
            
            MethodInfo method = inp.GetType().GetMethod("RenderForm");
            return (IInputSpecification<TypeViewModel>) method.Invoke(inp, new [] {controller, action});
        }

    }
}
