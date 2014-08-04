using System;
using System.Web.Mvc;
using BoC.Tasks;
using BoC.Web.Mvc.Binders;


namespace BoC.Web.Mvc.Init
{
    public class SetDefaultModelBinders : IBootstrapperTask
    {
        public void Execute()
        {
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
			ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
			ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

        }

    }
}