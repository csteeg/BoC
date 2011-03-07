using System.ComponentModel;
using BoC.InversionOfControl;

namespace BoC.ComponentModel.TypeExtension.DefaultSetupTasks
{
    public class RegisterTypeDescriptor : IContainerInitializer
    {
        public void Execute()
        {
            TypeDescriptor.AddProvider(new ExtendedTypeDescriptionProvider(typeof(object)), typeof(object));
        }

   }
}