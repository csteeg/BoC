using System.Collections.Generic;
using System.Linq;
using BoC.Helpers;
using Glass.Mapper.Configuration;
using Glass.Mapper.Configuration.Attributes;

namespace BoC.Persistence.SitecoreGlass.GlassConfig
{
    public class AppDomainHelperConfigLoader : IConfigurationLoader
    {
        private readonly IAppDomainHelper[] _helpers;
        public AppDomainHelperConfigLoader(IAppDomainHelper[] helpers)
        {
            _helpers = helpers;
        }

        public IEnumerable<AbstractTypeConfiguration> Load()
        {
            return
                _helpers.SelectMany(helper => helper.GetTypes(t => true),
                    (helper, type) => new AttributeTypeLoader(type))
                    .Select(loader => loader.Load().FirstOrDefault())
                    .Where(config => config != null)
                    .ToList();
        }
    }
}
