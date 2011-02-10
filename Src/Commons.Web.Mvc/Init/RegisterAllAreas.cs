using System.Web.Mvc;
using BoC.Tasks;

namespace BoC.Web.Mvc.Init
{
    public class RegisterAllAreas : IBootstrapperTask
    {
        public void Execute()
        {
            AreaRegistration.RegisterAllAreas();
        }
    }
}