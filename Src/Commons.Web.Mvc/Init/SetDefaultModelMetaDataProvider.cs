using System;
using System.ComponentModel;
using System.Web.Mvc;
using BoC.ComponentModel.TypeExtension;
using BoC.InversionOfControl;
using BoC.Tasks;
using BoC.Web.Mvc.MetaData;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaultModelMetaDataProvider : IContainerInitializer
    {
		public static bool SkipRegisterModelMetadataProvider { get; set; }
		public static bool SkipRegisterExtendedTypesRegistry { get; set; }

		private readonly IDependencyResolver dependencyResolver;

        public SetDefaultModelMetaDataProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            if (!SkipRegisterExtendedTypesRegistry && !(dependencyResolver.IsRegistered(typeof(IExtendedTypesRegistry))))
            {
                dependencyResolver.RegisterSingleton<IExtendedTypesRegistry, ExtendedTypesRegistry>();
            }
            if (!SkipRegisterModelMetadataProvider && !(dependencyResolver.IsRegistered(typeof(ModelMetadataProvider))))
            {
                dependencyResolver.RegisterSingleton<ModelMetadataProvider, ExtraModelMetadataProvider>();
            }
        }

   }
}