using System.Linq;
using System.Web.Mvc;
using BoC.Tasks;

namespace BoC.Sitecore.Mvc.Initialize
{
    public class RegisterValueProvider : IBootstrapperTask
    {
        public void Execute()
        {
            var index = ValueProviderFactories.Factories.FirstOrDefault(f => f is ChildActionValueProviderFactory);
            if (index != null)
            {
                ValueProviderFactories.Factories.Insert(ValueProviderFactories.Factories.IndexOf(index) + 1, new WildcardValueProviderFactory());
                ValueProviderFactories.Factories.Insert(ValueProviderFactories.Factories.IndexOf(index) + 1, new SitecoreValueProviderFactory());
            }
            else
            {
                ValueProviderFactories.Factories.Add(new SitecoreValueProviderFactory());
                ValueProviderFactories.Factories.Add(new WildcardValueProviderFactory());
            }
        }
    }
}