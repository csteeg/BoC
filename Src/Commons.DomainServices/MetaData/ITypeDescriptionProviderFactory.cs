using System.ComponentModel;

namespace BoC.DomainServices.MetaData
{
    public interface ITypeDescriptionProviderFactory
    {
        TypeDescriptionProvider CreateTypeDescriptionProvider(TypeDescriptionProvider existingProvider);
    }
}