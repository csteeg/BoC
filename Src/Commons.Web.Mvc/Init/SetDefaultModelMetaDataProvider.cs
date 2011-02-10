using System;
using System.Web.Mvc;
using BoC.InversionOfControl;
using BoC.Tasks;
using BoC.Web.Mvc.MetaData;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaultModelMetaDataProvider : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public SetDefaultModelMetaDataProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        private static Func<ModelMetadataProvider> _baseModelMetaDataProvider =
            () => new DataAnnotationsModelMetadataProvider();

        public static Func<ModelMetadataProvider> BaseModelMetaDataProvider
        {
            get { return _baseModelMetaDataProvider; }
            set { _baseModelMetaDataProvider = value; }
        }

        public void Execute()
        {
            if ((IoC.Resolver.IsRegistered(typeof (ModelMetadataProvider)))) 
                return;

            var instance = new ExtraAnnotationsModelMetaDataProvider(_baseModelMetaDataProvider());
            dependencyResolver.RegisterInstance<ModelMetadataProvider>(instance);
        }

   }
}