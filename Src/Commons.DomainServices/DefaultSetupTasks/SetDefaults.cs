using System;
using System.Linq;
using System.Web.DomainServices;
using System.Web.DynamicData;
using System.Web.DynamicData.ModelProviders;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;
using BoC.DomainServices;

namespace BoC.DomainServices.DefaultSetupTasks
{
    public class SetDefaults : IContainerInitializer
    {
        public void Execute()
        {
            IDomainServiceFactory factory;
            if (!IoC.IsRegistered<IDomainServiceFactory>())
            {
                factory = new IoCDomainServiceFactory();
                IoC.RegisterInstance<IDomainServiceFactory>(factory);
            }
            else
            {
                factory = IoC.Resolve<IDomainServiceFactory>();
            }
            DomainService.Factory = factory;

            if (!IoC.IsRegistered<MetaModel>())
            {
                IoC.RegisterInstance<MetaModel>(new MetaModel
                                                    {
                                                        DynamicDataFolderVirtualPath = "~/Views/Shared"
                                                    });
            }
        }
    }
}