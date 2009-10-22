using System;
using System.Configuration;
using BoC.Helpers;

namespace BoC.InversionOfControl
{
    public class DependencyResolverFactory : IDependencyResolverFactory
    {
        private readonly Type resolverType;

        public DependencyResolverFactory(string resolverTypeName)
        {
            Check.Argument.IsNotEmpty(resolverTypeName, "resolverTypeName");
            resolverType = Type.GetType(resolverTypeName, true, true);
        }

        public DependencyResolverFactory(Type resolverType)
        {
            Check.Argument.IsNotNull(resolverType, "resolverType");
            this.resolverType = resolverType;
        }

        public DependencyResolverFactory()
            : this(ConfigurationManager.AppSettings["dependencyResolverTypeName"])
        {
        }

        public IDependencyResolver CreateInstance()
        {
            return Activator.CreateInstance(resolverType) as IDependencyResolver;
        }
    }
}