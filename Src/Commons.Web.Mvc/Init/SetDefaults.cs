using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using BoC.InversionOfControl;
using BoC.Persistence;
using BoC.Tasks;
using BoC.Web.Mvc.Binders;
using MvcContrib.UI.InputBuilder;
using MvcContrib.UI.InputBuilder.Conventions;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaults : IBootstrapperTask
    {
        public void Execute()
        {
            //MvcContrib.UI.InputBuilder.InputBuilder.BootStrap();
            JqueryMvc.BootStrapper.Run();
            ControllerBuilder.Current.SetControllerFactory(typeof(AutoScaffoldControllerFactory));
            ModelBinders.Binders.DefaultBinder = new CommonModelBinder();
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            MvcContrib.UI.InputBuilder.InputBuilder.Conventions.Add(new ForeignKeyPropertyConvention());
        }
    }

    internal class ForeignKeyPropertyConvention : DefaultProperyConvention
    {
        /*
        private static readonly Type baseRepository = typeof(IRepository<>);

        public override bool CanHandle(PropertyInfo propertyInfo)
        {
            return (typeof(IBaseEntity).IsAssignableFrom(propertyInfo.PropertyType));
        }
        public override InputModelProperty ModelPropertyBuilder(PropertyInfo propertyInfo, object value)
        {
            if 
            {
                var repository = IoC.Resolve(baseRepository.MakeGenericType(propertyInfo.PropertyType)) as IQueryable;

                var selectList =
                    from entity in repository.OfType<IBaseEntity>()
                    select
                        new SelectListItem() { Text = entity.ToString(), Value = entity.Id + "", Selected = entity == value };

                return new ModelProperty<IEnumerable<SelectListItem>> { Value = selectList };

            }

            return base.ModelPropertyBuilder(propertyInfo, value);
        }
        
        public override string PartialNameConvention(PropertyInfo propertyInfo)
        {
            if (typeof(IBaseEntity).IsAssignableFrom(propertyInfo.PropertyType))
            {
                return "ForeignKey";
            }
            if (propertyInfo.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
            {
                var type = propertyInfo.PropertyType.GetGenericArguments()[0];
                if (typeof(IBaseEntity).IsAssignableFrom(type))
                {
                    return "Children";
                }
            }

            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return PartialNameConvention(propertyInfo.PropertyType.GetProperty("Value"));
            }
            if (propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(Byte) && propertyInfo.PropertyType != typeof(sbyte)
                && propertyInfo.PropertyType != typeof(Boolean) && propertyInfo.PropertyType != typeof(char))
            {
                return "Int32";
            }

            return base.PartialNameConvention(propertyInfo);
        }
        */
    }
}