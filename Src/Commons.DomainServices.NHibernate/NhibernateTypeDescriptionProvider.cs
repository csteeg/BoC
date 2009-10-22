using System;
using System.ComponentModel;
using NHibernate.Cfg;

namespace BoC.DomainServices.NHibernate
{
    /// <summary>
    /// Wraps an existing typedescriptionprovider to add a validationattribute to all types
    /// </summary>
    public class NhibernateTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly Configuration nHibernateConfiguration;

        public NhibernateTypeDescriptionProvider(TypeDescriptionProvider parent, Configuration nHibernateConfiguration): base(parent)
        {
            this.nHibernateConfiguration = nHibernateConfiguration;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new NhibernateTypeDescriptor(objectType, base.GetTypeDescriptor(objectType, instance), nHibernateConfiguration);
        }
    }
}