using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BoC.Helpers;

namespace BoC.InversionOfControl
{
    public static class IoC
    {
        private static readonly object resolverLock = new object();

        public static void InitializeWith(IDependencyResolver resolver)
        {
            Check.Argument.IsNotNull(resolver, "resolver");
            lock (resolverLock)
            {
                if (IoC.Resolver != null)
                    throw new ArgumentException("IoC already initialized");
                
                IoC.Resolver = resolver;

                RunContainerInitailizers(resolver);
            }
        }

        public static bool IsInitialized()
        {
            return Resolver != null;
        }

        private static IDependencyResolver resolver;
        public static IDependencyResolver Resolver
        {
            get { return resolver; }
            set
            {
                lock (resolverLock)
                {
                    resolver = value;
                }
            }
        }

        private static void RunContainerInitailizers(IDependencyResolver dependencyResolver)
        {
            string binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            // In the context of a unit test the privatebinpath is an empty string.
            if (String.IsNullOrEmpty(binPath))
            {
                binPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }

            foreach (var dir in binPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var file in Directory.GetFiles(dir, "*.dll"))
                {
                    try
                    {
                        Assembly.LoadFrom(file);
                    }
                    catch { }
                }
            }
            try
            {
                var initTasks =
                    AppDomain.CurrentDomain.GetAssemblies().ToList()
                        .SelectMany(s => s.GetTypes())
                        .Where(t => t.IsClass && !t.IsAbstract && typeof(IContainerInitializer).IsAssignableFrom(t))
                        .ToList();

                foreach (var t in initTasks.Where(t => !t.Namespace.StartsWith("BoC."))) //first user's tasks
                {
                    RunContainerInitailizer(t, dependencyResolver);
                }
                foreach (var t in initTasks.Where(t => t.Namespace.StartsWith("BoC."))) //now ours
                {
                    RunContainerInitailizer(t, dependencyResolver);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                StringBuilder message = new StringBuilder("Failed to load assembly.\nFailed types:\n");
                message.AppendLine("---------------------------");
                foreach (var type in e.Types)
                {
                    message.AppendLine(type + "");
                }
                message.AppendLine("---------------------------");
                message.AppendLine("LoaderExceptions:");
                message.AppendLine("---------------------------");
                foreach (var exception in e.LoaderExceptions)
                {
                    message.AppendLine(exception.Message);
                }
                throw new Exception(message.ToString(), e);
            }
        }

        private static void RunContainerInitailizer(Type type, IDependencyResolver dependencyResolver)
        {
            IContainerInitializer initializer = null;
            try
            {
                var constructor = type.GetConstructor(new[] {typeof (IDependencyResolver)});
                if (constructor != null)
                {
                    initializer = Activator.CreateInstance(type,new object[]{ dependencyResolver}) as IContainerInitializer;
                }
            }
            catch {}

            if (initializer == null)
                initializer = Activator.CreateInstance(type) as IContainerInitializer;
            
            if (initializer != null)
                initializer.Execute();
        }
    }
}