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
            if (!ModelBinders.Binders.ContainsKey(typeof(DateTime)))
                ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            if (!ModelBinders.Binders.ContainsKey(typeof(DateTime?)))
                ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            if (!ModelBinders.Binders.ContainsKey(typeof(decimal)))
                ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            if (!ModelBinders.Binders.ContainsKey(typeof(decimal?)))
                ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

        }

    }
}