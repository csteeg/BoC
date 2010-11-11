﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace BoC.InverionOfControl.Unity
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
            var container = context.NewBuildUp<IUnityContainer>();
            var firstItem = container.Resolve<T>();
            var results = new List<T>(container.ResolveAll<T>());
            if (firstItem != null)
                results.Insert(0, firstItem);
            return results.ToArray();
        }
    }
}
