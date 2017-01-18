using System.Linq;
using BoC.InversionOfControl;
using Glass.Mapper.Configuration;
using Glass.Mapper.IoC;
using Glass.Mapper.Maps;
using Glass.Mapper.Pipelines.ObjectConstruction.Tasks.CacheCheck;
using Glass.Mapper.Sc.CodeFirst;
using Glass.Mapper.Sc.ContentSearch.Pipelines.ObjectConstruction.Tasks.SearchProxy;
using Glass.Mapper.Sc.IoC;
using Sitecore.SecurityModel;
using IDependencyResolver = Glass.Mapper.Sc.IoC.IDependencyResolver;

namespace $rootnamespace$.App_Start
{
	//Comment out all code in glass.mapper's original GlassMapperScCustom.cs and move your custom code to here. This version has a different CreateResolver() for contentsearch support
    public static  class GlassMapperScCustom
    {
		public static IDependencyResolver CreateResolver(){
			Glass.Mapper.Configuration.Defaults.OverallConfiguration.InferType = true;
			Glass.Mapper.Configuration.Defaults.OverallConfiguration.IsLazy = true;
			global::BoC.Persistence.SitecoreGlass.Initialize.InitBoc.Start();

			var config = new global::Glass.Mapper.Sc.Config();
			config.AutoImportBaseClasses = true;
			config.UseProxiesForLazyEnumerables = true;

			var resolver = new DependencyResolver(config);
			resolver.DataMapperResolverFactory.Add(() => new AbstractDataMapperFieldsWithSpace());
			//trigger addtypes
			var items = resolver.ObjectConstructionFactory.GetItems().ToArray();
			var index = 0;
			for (var i=0;i<items.Length;i++)
			{
				if (items[i] is CacheCheckTask) {
					index = i+1;
					break;
				}
			}
			resolver.ObjectConstructionFactory.Insert(index, () => new SearchProxyWrapperTask());
			return resolver;
		}

		public static IConfigurationLoader[] GlassLoaders(){			
			
			/* Register any ConfigurationLoader you use (eg fluent) in the IoC.Resolver.
             * 
             * If you are using Attribute Configuration or automapping/on-demand mapping you don't need to do anything!
             * 
             */

			return global::BoC.InversionOfControl.IoC.Resolver.ResolveAll<IConfigurationLoader>().ToArray();
		}
		public static void PostLoad(){
			global::BoC.Profiling.Profiler.Enabled = global::Sitecore.Configuration.Settings.GetBoolSetting("BoC.Profiler.Enabled", false);
			//Set config property to true in Glass.Mapper.Sc.CodeFirst.config to enable codefirst
			if (!global::Sitecore.Configuration.Settings.GetBoolSetting("Glass.CodeFirst", false)) return;

            var dbs = global::Sitecore.Configuration.Factory.GetDatabases();
            foreach (var db in dbs)
            {
                var provider = db.GetDataProviders().FirstOrDefault(x => x is GlassDataProvider) as GlassDataProvider;
                if (provider != null)
                {
                    using (new SecurityDisabler())
                    {
                        provider.Initialise(db);
                    }
                }
            }
		}
		public static void AddMaps(IConfigFactory<IGlassMap> mapsConfigFactory)
        {
			// Add maps here
            // mapsConfigFactory.Add(() => new SeoMap());
        }
    }
}
