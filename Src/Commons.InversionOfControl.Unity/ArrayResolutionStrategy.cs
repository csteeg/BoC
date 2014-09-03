using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace BoC.InversionOfControl.Unity
{
    /// <summary>
    /// This strategy implements the logic that will call container.ResolveAll
    /// when an array parameter is detected.
    /// </summary>
    public class CustomArrayResolutionStrategy : BuilderStrategy
    {
        private delegate object ArrayResolver(IBuilderContext context);
        private static readonly MethodInfo genericResolveArrayMethod = typeof(CustomArrayResolutionStrategy)
                .GetMethod("ResolveArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        /// <summary>
        /// Do the PreBuildUp stage of construction. This is where the actual work is performed.
        /// </summary>
        /// <param name="context">Current build context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            Type typeToBuild = context.BuildKey.Type;
            if (typeToBuild.IsArray && typeToBuild.GetArrayRank() == 1)
            {
                Type elementType = typeToBuild.GetElementType();

                MethodInfo resolverMethod = genericResolveArrayMethod.MakeGenericMethod(elementType);

                ArrayResolver resolver = (ArrayResolver)Delegate.CreateDelegate(typeof(ArrayResolver), resolverMethod);

                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        private static object ResolveArray<T>(IBuilderContext context)
        {
            IUnityContainer container = context.NewBuildUp<IUnityContainer>();

            var registrations = container.Registrations;

            if (typeof(T).IsGenericType)
            {
                registrations = registrations
                    .Where(registration => registration.RegisteredType == typeof(T)
                                           || registration.RegisteredType == typeof(T).GetGenericTypeDefinition());
            }
            else
            {
                registrations = registrations
                    .Where(registration => registration.RegisteredType == typeof(T));
            }

            var registeredNames = registrations
                .Select(registration => registration.Name) // note: including empty ones
                .Distinct()
                .ToList();

            var results = registeredNames
                .Select(name => context.NewBuildUp<T>(name))
                .ToArray();

            return results;
        }
    }
}
