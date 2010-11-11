using System;
using BoC;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;

namespace NorthwindMvcScaffold.Models.Mappings
{
    public class HasyManyConvention: IHasManyConvention
    {
        public void Apply(IOneToManyCollectionInstance target)
        {
            if (target.Key.Columns.IsEmpty())
            {
                target.Key.Column(target.EntityType.Name + "ID");
                //target.AsSet();
                target.Key.ForeignKey("FK_" + target.Member.Name + "_"
                                                    + Inflector.Pluralize(target.EntityType.Name));
            }
        }
    }
    public class IdConvention: IIdConvention
    {
        public void Apply(IIdentityInstance target)
        {
            target.Column(target.EntityType.Name + "ID");
            if (target.Property.PropertyType == typeof(string))
            {
                target.UnsavedValue(null);
                target.GeneratedBy.Assigned();
            }
            else if (target.Property.PropertyType.IsValueType)
            {
                target.UnsavedValue("0");
                target.GeneratedBy.Identity();
            }
        }

    }
    public class ReferenceConvention: IReferenceConvention
    {
        public void Apply(IManyToOneInstance target)
        {
            if (target.Property.Name == target.Property.PropertyType.Name)
            {
                target.Column(target.Property.Name + "ID");
            }
            else
            {
                target.Column(target.Property.Name);
            }
        }

    }
    public class ClassConvention: IClassConvention
    {
        public void Apply(IClassInstance target)
        {
            target.Table(Inflector.Pluralize(target.EntityType.Name));
        }

    }
}