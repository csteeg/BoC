using System;
using System.ComponentModel;
using System.Web.DomainServices;
using BoC.InversionOfControl;

namespace BoC.DomainServices.MetaData
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class IoCMetaDataProviderAttribute : MetadataProviderAttribute
    {
        public IoCMetaDataProviderAttribute(): base(typeof(TypeDescriptionProvider))
        {
        }

        /// <summary>
        /// This method creates an instance of the <see cref="TypeDescriptionProvider"/>.
        /// </summary>
        /// <param name="existingProvider">The existing <see cref="TypeDescriptionProvider"/> for the Type the returned
        /// provider will be registered for.</param>
        /// <param name="domainServiceType">The <see cref="DomainService"/> Type metadata is being provided for.</param>
        /// <returns>The <see cref="TypeDescriptionProvider"/> instance.</returns>
        public override TypeDescriptionProvider CreateProvider(TypeDescriptionProvider existingProvider, Type domainServiceType)
        {
            if (domainServiceType == null)
            {
                throw new ArgumentNullException("domainServiceType");
            }
            if (IoC.IsRegistered<ITypeDescriptionProviderFactory>())
            {
                var typeDescriptionProvider = IoC.Resolve<ITypeDescriptionProviderFactory>();
                return typeDescriptionProvider.CreateTypeDescriptionProvider(existingProvider);
            }
            return existingProvider;
        }

    }
}