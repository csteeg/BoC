using System;
using System.Linq;
using System.Web.Mvc;
using BoC.Validation;

namespace BoC.Web.Mvc.Validation
{
    public static class RulesExceptionExtensions
    {
        public static void AddModelStateErrors(this RulesException exception, ModelStateDictionary modelState)
        {
            AddModelStateErrors(exception, modelState, null, x => true);
        }

        public static void AddModelStateErrors(this RulesException exception, ModelStateDictionary modelState, string prefix)
        {
            AddModelStateErrors(exception, modelState, prefix, x => true);
        }

        public static void AddModelStateErrors(this RulesException exception, ModelStateDictionary modelState, string prefix, Func<ErrorInfo, bool> errorFilter)
        {
            if (errorFilter == null) throw new ArgumentNullException("errorFilter");
            prefix = prefix == null ? "" : prefix + ".";
            foreach (var errorInfo in exception.Errors.Where(errorFilter))
            {
                var key = prefix + errorInfo.PropertyName;
                modelState.AddModelError(key, errorInfo.ErrorMessage);

                ModelState existingModelStateValue;
                if (modelState.TryGetValue(key, out existingModelStateValue) && existingModelStateValue.Value == null)
                    existingModelStateValue.Value = new ValueProviderResult(null, null, null);
            }
        }
    }
}
