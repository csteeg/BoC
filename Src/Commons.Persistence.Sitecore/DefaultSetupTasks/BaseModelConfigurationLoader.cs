using System;
using System.Collections.Generic;
using System.Linq;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.Models;
using BoC.Tasks;
using Glass.Mapper.Configuration;
using Glass.Mapper.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Fluent;
using Sitecore.Events;

namespace BoC.Persistence.SitecoreGlass.GlassConfig
{
	public class BaseModelConfigurationLoader : IContainerInitializer
	{
	  public static bool Disabled {get;set;}
    private readonly IDependencyResolver _resolver;

		public BaseModelConfigurationLoader(IDependencyResolver resolver)
		{
			_resolver = resolver;
		}

		public void Execute()
		{
      if (Disabled)
        return;
			var loader = new SitecoreFluentConfigurationLoader();

			var isitecoreItemConfig = loader.Add<ISitecoreItem>().TemplateId(new Guid("{1930BBEB-7805-471A-A3BE-4858AC7CF696}")).AutoMap();
			isitecoreItemConfig.Info(x => x.TemplateId).InfoType(SitecoreInfoType.TemplateId);
			isitecoreItemConfig.Info(x => x.SitecorePath).InfoType(SitecoreInfoType.FullPath);
			isitecoreItemConfig.Info(x => x.Name).InfoType(SitecoreInfoType.Name);
			isitecoreItemConfig.Info(x => x.BaseTemplateIds).InfoType(SitecoreInfoType.BaseTemplateIds);
			isitecoreItemConfig.Info(x => x.Language).InfoType(SitecoreInfoType.Language);
			isitecoreItemConfig.Field(x => x.SortOrder).FieldId(new Guid("{BA3F86A2-4A1C-4D78-B63D-91C2779C1B5E}")).FieldType(SitecoreFieldType.Integer).SectionName("Name");
			isitecoreItemConfig.Field(x => x.Icon).FieldId(new Guid("{06D5295C-ED2F-4A54-9BF2-26228D113318}")).Configuration.CodeFirst = false;

			loader.Add<ISearchable>().CodeFirst().TemplateId(new Guid("{41d82537-4720-409b-903e-2bb2f64312f2}"));

			var sitecoreItemConfig  = loader.Add<SitecoreItem>().TemplateId(new Guid("{1930BBEB-7805-471A-A3BE-4858AC7CF696}")).AutoMap();
			sitecoreItemConfig.Import(isitecoreItemConfig);

			_resolver.RegisterInstance<IConfigurationLoader>(loader);
		}
	}
}
