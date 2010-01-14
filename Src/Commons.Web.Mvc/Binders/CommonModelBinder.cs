using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
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

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            BindEntityModel(controllerContext, bindingContext);
            return base.BindModel(controllerContext, bindingContext);
        }

        private void BindEntityModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            if (typeof(IBaseEntity).IsAssignableFrom(modelType))
            {
                //if we somehow already have the entity in the routedata (eg. Html.Action)
                var rawValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (rawValue != null && rawValue is IBaseEntity)
                {
                    return;
                }

                var modelName = (bindingContext.ModelName == "entity") ? null : bindingContext.ModelName;
                var valueName = CreateSubPropertyName(modelName, "Id");

                ValueProviderResult value = bindingContext.ValueProvider.GetValue(valueName);
                if (value != null)
                {
                    var toProp = modelType.GetProperty("Id");
                    object pkValue = value.ConvertTo(toProp.PropertyType);
                    if (pkValue != null)
                    {
                        //we have a primary key value, let's get the service
                        var serviceType = typeof(IModelService<>).MakeGenericType(modelType);
                        if (IoC.IsRegistered(serviceType))
                        {
                            var service = IoC.Resolve(serviceType) as IModelService;
                            if (service != null)
                            {
                                bindingContext.ModelMetadata.Model = service.Get(pkValue);
                            }
                        }
                    }
                }

            }
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
        protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            return true;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }

        /// <summary>
        /// If the property being bound is a simple, generic collection of entiy objects, then use 
        /// reflection to get past the protected visibility of the collection property, if necessary.
        /// </summary>
        private void UpdateEntityCollectionModel(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            // need to skip properties that aren't part of the request, else we might hit a StackOverflowException
            string fullPropertyKey = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
            if (!bindingContext.ValueProvider.ContainsPrefix(fullPropertyKey))
            {
                return;
            }

            var collection = propertyDescriptor.GetValue(bindingContext.Model) as IEnumerable;
            if (collection != null)
                //STUPID HashSet is not inheriting from ICollection :(( warning, reflection ahead and ASSUMING add & remove methods are available on the collection :(
            {
                var entityType = propertyDescriptor.PropertyType.GetGenericArguments().First();
                var serviceType = typeof(IModelService<>).MakeGenericType(entityType);
                var idProp = entityType.GetProperty("Id");
                var service = IoC.Resolve(serviceType) as IModelService;

                var valueName = CreateSubPropertyName(fullPropertyKey, "Id");
                var incoming_raw = bindingContext.ValueProvider.GetValue(valueName) ?? bindingContext.ValueProvider.GetValue(fullPropertyKey);
                
                var incoming = incoming_raw.RawValue as IEnumerable;
                if (incoming == null && incoming_raw.RawValue != null)
                {
                    incoming = new[] {incoming_raw.RawValue};
                }

                var query = incoming.AsQueryable().Cast<string>();
                var collectionArray = collection.Cast<IBaseEntity>().ToList();
                //first see if the current objects are posted again.
                foreach (var entity in collectionArray)
                {
                    if (!query.Any(s => s == entity.Id + ""))
                    {
                        propertyDescriptor.PropertyType.InvokeMember("Remove", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, collection, new object[] { entity });
                    }
                }

                query = query.Except(collectionArray.Select(e => e.Id + ""));
                //now see if there are new object-id's to add
                foreach (var id in query)
                {
                    object idValue = null;
                    if (idProp.PropertyType != typeof(string) && (id.Contains("{") || id.Contains("<")))
                    {
                        continue; //skip stuff that's problem 
                    }

                    try { idValue = Convert.ChangeType(id, idProp.PropertyType); }
                    catch {}
                    if (idValue != null)
                    {
                        var obj = service.Get(idValue);
                        if (obj != null)
                        {
                            propertyDescriptor.PropertyType.InvokeMember("Add",
                                                                         BindingFlags.Public | BindingFlags.Instance |
                                                                         BindingFlags.InvokeMethod,
                                                                         null,
                                                                         collection,
                                                                         new[] {obj});
                            
                            query = query.Where(s => s != idValue);
                        }
                    }
                }

                //we have some new objects to be created
                if (query.Any())
                {
                    foreach (var value in query)
                    {
                        object obj = null;
                        if (value.StartsWith("<"))
                        {
                            try
                            {
                                var xmlSerializer = new XmlSerializer(entityType);
                                obj = xmlSerializer.Deserialize(new StringReader(value));
                            }
                            catch {}
                        }
                        if (obj == null)
                        {
                            try
                            {
                                var jsonSerializer = new JavaScriptSerializer();
                                var deserialize = typeof(JavaScriptSerializer).GetMethod("Deserialize").MakeGenericMethod(entityType);
                                obj = deserialize.Invoke(jsonSerializer, new[] { value });
                            }
                            catch {}
                        }
                        if (obj != null)
                        {
                            propertyDescriptor.PropertyType.InvokeMember("Add",
                                                                         BindingFlags.Public | BindingFlags.Instance |
                                                                         BindingFlags.InvokeMethod,
                                                                         null,
                                                                         collection,
                                                                         new[] {obj});
                        }
                    }
                }
            }
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (IsBindableEntityCollection(propertyDescriptor.PropertyType))
            {
                UpdateEntityCollectionModel(controllerContext, bindingContext, propertyDescriptor);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
            
        }

        private static bool IsBindableEntityCollection(Type propertyType)
        {
            return propertyType.IsGenericType 
                    && typeof(ICollection<>).IsAssignableFrom(propertyType.GetGenericTypeDefinition())
                    && IsEntityType(propertyType.GetGenericArguments().First());
        }

        private static bool IsEntityType(Type propertyType)
        {
            return typeof(IBaseEntity).IsAssignableFrom(propertyType);
        }
    }
}
