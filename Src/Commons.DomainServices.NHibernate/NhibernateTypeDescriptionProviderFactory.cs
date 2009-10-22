using System.ComponentModel;
using BoC.DomainServices.MetaData;
using NHibernate;
using NHibernate.Cfg;

namespace BoC.DomainServices.NHibernate
{
    public class NhibernateTypeDescriptionProviderFactory : ITypeDescriptionProviderFactory
    {
        private readonly Configuration nhibernateConfig;

        public NhibernateTypeDescriptionProviderFactory(Configuration nhibernateConfig)
        {
            this.nhibernateConfig = nhibernateConfig;
        }

        public TypeDescriptionProvider CreateTypeDescriptionProvider(TypeDescriptionProvider existingProvider)
        {
            return new NhibernateTypeDescriptionProvider(existingProvider, nhibernateConfig);
        }
    }
}