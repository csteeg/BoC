using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace BoC.InversionOfControl.SimpleInjector
{
  public class AllowMultipleConstructorResolutionBehavior :  IConstructorResolutionBehavior
  {
    public ConstructorInfo GetConstructor(Type serviceType, Type implementationType)
    {
      VerifyTypeIsConcrete(implementationType);
      return GetSinglePublicConstructor(implementationType);
    }

    private static void VerifyTypeIsConcrete(Type implementationType)
    {
      if (!implementationType.IsAbstract && !implementationType.IsArray && implementationType != typeof(object) && !typeof(Delegate).IsAssignableFrom(implementationType))
        return;
      throw new ActivationException(string.Format("Type {0} should be concrete", implementationType));
    }

    private static ConstructorInfo GetSinglePublicConstructor(Type implementationType)
    {
      return (
          from ctor in implementationType.GetConstructors()
          orderby ctor.GetParameters().Length descending
          select ctor)
         .First();
    }
  }
}
