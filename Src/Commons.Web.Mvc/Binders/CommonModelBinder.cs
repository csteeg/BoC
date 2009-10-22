using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Dynamic;
using System.Web.DynamicData;
using System.Web.Mvc;
using BoC.InversionOfControl;
using BoC.Linq;
using BoC.Persistence;
using BoC.Services;

namespace BoC.Web.Mvc.Binders
{
    public class CommonModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// After the model is updated, there may be a number of ModelState errors added by ASP.NET MVC for 
        /// and data casting problems that it runs into while binding the object.  This gets rid of those
        /// casting errors
        /// </summary>
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            foreach (string key in bindingContext.ModelState.Keys)
            {
                for (int i = 0; i < bindingContext.ModelState[key].Errors.Count; i++)
                {
                    ModelError modelError = bindingContext.ModelState[key].Errors[i];

                    // Get rid of all the MVC errors except those associated with parsing info; e.g., parsing DateTime fields
                    if (IsModelErrorAddedByMvc(modelError) && !IsMvcModelBinderFormatException(modelError))
                    {
                        bindingContext.ModelState[key].Errors.RemoveAt(i);
                        // Decrement the counter since we've shortened the list
                        i--;
                    }
                }
            }
        }

        private bool IsModelErrorAddedByMvc(ModelError modelError)
        {
            return modelError.Exception != null &&
                modelError.Exception is InvalidOperationException;
        }

        private bool IsMvcModelBinderFormatException(ModelError modelError)
        {
            return modelError.Exception != null &&
                   modelError.Exception.InnerException != null &&
                   (
                       modelError.Exception.InnerException is FormatException
                       ||
                       (modelError.Exception.InnerException.InnerException != null && modelError.Exception.InnerException.InnerException is FormatException)
                   );
        }

        /// <summary>
        /// The base implementation of this uses IDataErrorInfo to check for validation errors and 
        /// adds them to the ModelState. This override prevents that from occurring by doing nothing at all.
        /// </summary>
        protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
        }

        /// <summary>
        /// The base implementatoin of this looks to see if a property value provided via a form is 
        /// bindable to the property and adds an error to the ModelState if it's not.  For example, if 
        /// a text box is left blank and the binding property is of type int, then the base implementation
        /// will add an error with the message "A value is required." to the ModelState.  We don't want 
        /// this to occur as we want these type of validation problems to be verified by our business rules.
        /// </summary>
        protected override bool OnPropertyValidating(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            return true;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            Type type = modelType;
            if (modelType.IsGenericType)
            {
                Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IDictionary<,>))
                {
                    type = typeof(Dictionary<,>).MakeGenericType(modelType.GetGenericArguments());
                }
                else if (((genericTypeDefinition == typeof(IEnumerable<>)) || (genericTypeDefinition == typeof(ICollection<>))) || (genericTypeDefinition == typeof(IList<>)))
                {
                    type = typeof(List<>).MakeGenericType(modelType.GetGenericArguments());
                    return Activator.CreateInstance(type);
                }
            }
            return IoC.Resolve(type);
        }

        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            if (modelType.IsClass && IoC.IsRegistered<MetaModel>())
            {
                var model = IoC.Resolve<MetaModel>();
                MetaTable table;
                if (model.TryGetTable(modelType, out table))
                {
                    var query = table.GetQuery();
                    var pk = table.PrimaryKeyColumns[0].Name;
                    var modelName = (bindingContext.ModelName == "entity") ? null : bindingContext.ModelName;
                    var valueName = CreateSubPropertyName(modelName, pk);

                    ValueProviderResult value;
                    if (bindingContext.ValueProvider.TryGetValue(valueName, out value))
                    {
                        object pkValue = value.AttemptedValue;
                        if (pkValue != null)
                        {
                            //bindingContext.ValueProvider.Remove(valueName);
                            try
                            {
                                bindingContext.Model = query.Where(table.PrimaryKeyColumns[0] + " == @0", pkValue)
                                        .FirstOrDefault();
                            }
                            catch (FormatException)
                            {
                                //could have an invalid keyvalue submitted
                            }
                        }
                    }
                }
            }
            else if (typeof(IBaseEntity).IsAssignableFrom(modelType))
            {
                var modelName = (bindingContext.ModelName == "entity") ? null : bindingContext.ModelName;
                var valueName = CreateSubPropertyName(modelName, "Id");

                ValueProviderResult value;
                if (bindingContext.ValueProvider.TryGetValue(valueName, out value))
                {
                    object pkValue = value.ConvertTo(typeof(int));
                    if (pkValue != null)
                    {
                        //we have a primary key value, let's get the service
                        var serviceType = typeof (IModelService<>).MakeGenericType(modelType);
                        if (IoC.IsRegistered(serviceType))
                        {
                            var service = IoC.Resolve(serviceType) as IRepository;
                            if (service != null)
                            {
                                bindingContext.Model = service.Get(pkValue);
                            }
                        }
                    }
                }

            }

            return base.BindModel(controllerContext, bindingContext);
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}
