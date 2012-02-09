using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Binders
{
	public class DecimalModelBinder : DefaultModelBinder
	{
		public override object BindModel(
					ControllerContext controllerContext, 
					ModelBindingContext bindingContext)
		{
			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueProviderResult.AttemptedValue.Equals("N.aN") || valueProviderResult.AttemptedValue.Equals("NaN") || valueProviderResult.AttemptedValue.Equals("Infini.ty") || valueProviderResult.AttemptedValue.Equals("Infinity"))
				return 0m;
			return valueProviderResult == null ? base.BindModel(controllerContext, bindingContext) : Convert.ToDecimal(valueProviderResult.AttemptedValue);
		}

	}
}
