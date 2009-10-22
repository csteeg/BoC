using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Type;

namespace BoC.DomainServices.NHibernate
{
    class NhibernateTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Type entityType;
        private readonly Configuration nhibernateConfiguration;
        private PersistentClass classMetadata;

        public NhibernateTypeDescriptor(Type entityType, ICustomTypeDescriptor parent, Configuration nhibernateConfiguration)
            : base(parent)
        {
            while (entityType != null && entityType.Name.EndsWith("Proxy") && entityType.Assembly.GetName().Name.EndsWith("ProxyAssembly"))
                entityType = entityType.BaseType;
            this.entityType = entityType;
            this.nhibernateConfiguration = nhibernateConfiguration;
            this.classMetadata = nhibernateConfiguration.GetClassMapping(this.entityType);
        }

        private PropertyDescriptorCollection properties = null;
        public override PropertyDescriptorCollection GetProperties()
        {
            if (properties == null)
            {
                properties = base.GetProperties();
                bool hasEntityAttributes = false;
                var list = new List<PropertyDescriptor>();
                foreach (PropertyDescriptor descriptor in properties)
                {
                    Attribute[] attrs = this.GetEntityMemberAttributes(descriptor).ToArray<Attribute>();
                    if (attrs.Length > 0)
                    {
                        list.Add(new PropertyDescriptorWrapper(descriptor, attrs));
                        hasEntityAttributes = true;
                    }
                    else
                    {
                        list.Add(descriptor);
                    }
                }
                if (hasEntityAttributes)
                {
                    properties = new PropertyDescriptorCollection(list.ToArray(), true);
                }
            }
            return properties;
        }

        public override AttributeCollection GetAttributes()
        {
            var attributes = base.GetAttributes();
            if (classMetadata == null)
            {
                var newattributes = new List<Attribute>(attributes.Cast<Attribute>());
                newattributes.Add(new ScaffoldTableAttribute(false));
                return new AttributeCollection(newattributes.ToArray());
            }
            return attributes;
        }


        protected virtual IEnumerable<Attribute> GetEntityMemberAttributes(PropertyDescriptor propertyDescriptor)
        {
            if (classMetadata == null)
                return null;

            Property member;
            try
            {
                member = classMetadata.GetProperty(propertyDescriptor.Name);
            }
            catch (MappingException)
            {
                return new Attribute[] { new ScaffoldColumnAttribute(false) };
            }

            List<Attribute> attributes = new List<Attribute>();
            if (classMetadata.Identifier != null)
            {
                foreach (Column id in classMetadata.Identifier.ColumnIterator)
                {
                    if (id.Name == propertyDescriptor.Name)
                    {
                        if (propertyDescriptor.Attributes[typeof (ScaffoldColumnAttribute)] == null)
                        {
                            //override the default behaviour of not scaffolding properties being used in a foreign key
                            attributes.Add(new ScaffoldColumnAttribute(true));
                        }
                        if (propertyDescriptor.Attributes[typeof (KeyAttribute)] == null)
                        {
                            attributes.Add(new KeyAttribute());
                        }
                        if (propertyDescriptor.Attributes[typeof (EditableAttribute)] == null)
                        {
                            var editable = new EditableAttribute(false);
                            if (id.Value is SimpleValue)
                                editable.AllowInitialValue =
                                    "assigned".Equals(((SimpleValue) id.Value).IdentifierGeneratorStrategy,
                                                      StringComparison.InvariantCultureIgnoreCase);
                            attributes.Add(editable);
                        }
                        break;
                    }
                }
            }
            if (member != null)
            {
                if ((!member.IsNullable) &&
                    (!propertyDescriptor.PropertyType.IsValueType &&
                     (propertyDescriptor.Attributes[typeof (RequiredAttribute)] == null)))
                {
                    attributes.Add(new RequiredAttribute());
                }
                if (member.Generation != PropertyGeneration.Never && (propertyDescriptor.Attributes[typeof(ScaffoldColumnAttribute)] == null))
                {
                    attributes.Add(new ScaffoldColumnAttribute(false));
                }
                if (member.Type.IsAssociationType && (propertyDescriptor.Attributes[typeof (AssociationAttribute)] == null))
                {
                    String name = null;
                    string otherkey = "Id";
                    if (member.Type.IsCollectionType)
                    {
                        name = ((Collection) member.Value).Element.Type.Name + "." +
                               ((Collection) member.Value).Key.ColumnIterator.First().Text;
                        otherkey =
                            nhibernateConfiguration.GetClassMapping(
                                ((Collection) member.Value).Element.Type.ReturnedClass).IdentifierProperty.Name;
                    }
                    else
                    {
                        name = classMetadata.MappedClass.FullName + "." + member.ColumnIterator.First().Text;
                        otherkey =
                            nhibernateConfiguration.GetClassMapping(member.Type.ReturnedClass).IdentifierProperty.Name;
                    }
                    var attribute = new AssociationAttribute(
                        name,
                        classMetadata.IdentifierProperty.Name,
                        otherkey
                        );
                    var fromParent = ForeignKeyDirection.ForeignKeyFromParent.GetType();
                    attribute.IsForeignKey =
                        fromParent.IsAssignableFrom(((IAssociationType) member.Type).ForeignKeyDirection.GetType());
                    attributes.Add(attribute);
                }
                //if ((member.UpdateCheck != UpdateCheck.Never) &&
                //    (propertyDescriptor.Attributes[typeof (ConcurrencyCheckAttribute)] == null))
                //{
                //    attributes.Add(new ConcurrencyCheckAttribute());
                //}
                //if (member.IsVersion && (propertyDescriptor.Attributes[typeof (TimestampAttribute)] == null))
                //{
                //    attributes.Add(new TimestampAttribute());
                //}
                //if ((((propertyDescriptor.PropertyType == typeof (string)) ||
                //      (propertyDescriptor.PropertyType == typeof (char[]))) && (member.DbType != null)) &&
                //    ((member.DbType.Length > 0) && (propertyDescriptor.Attributes[typeof (StringLengthAttribute)] == null)))
                //{
                //    InferStringLengthAttribute(member.DbType, attributes);
                //}
            }
            return attributes.ToArray();
        }
    }
}