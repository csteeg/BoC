using System;
using System.Linq;
using BoC.DomainServices;
using BoC.DomainServices.MetaData;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;

namespace BoC.DomainServices.NHibernate.DefaultSetupTasks
{
    public class SetDefaults : IContainerInitializer
    {
        public void Execute()
        {
            if (!IoC.IsRegistered<ITypeDescriptionProviderFactory>())
            {
                IoC.RegisterSingleton<ITypeDescriptionProviderFactory,NhibernateTypeDescriptionProviderFactory>();
            }
        }
    }
}