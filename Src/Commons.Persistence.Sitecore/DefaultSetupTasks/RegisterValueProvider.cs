using System.Linq;
using System.Web.Mvc;
using BoC.Tasks;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{
    public class RegisterValueProvider : IBootstrapperTask
    {
        public void Execute()
        {
            var index = ValueProviderFactories.Factories.FirstOrDefault(f => f is ChildActionValueProviderFactory);
            if (index != null)
            {
                ValueProviderFactories.Factories.Insert(ValueProviderFactories.Factories.IndexOf(index) + 1, new ParametersTemplateValueProviderFactory());
            }
            else
            {
                ValueProviderFactories.Factories.Add(new ParametersTemplateValueProviderFactory());
            }
        }
    }
}