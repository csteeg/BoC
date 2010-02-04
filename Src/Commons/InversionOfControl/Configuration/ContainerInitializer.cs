using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BoC.Extensions;
using BoC.Tasks;

namespace BoC.InversionOfControl.Configuration
{
    public static class ContainerInitializer
    {
        public static void Execute()
        {
            string binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            // In the context of a unit test the privatebinpath is an empty string.
            if(String.IsNullOrEmpty(binPath))
            {
                binPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }

            foreach (var dir in binPath.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var file in Directory.GetFiles(dir, "*.dll"))
                {
                    try
                    {
                        Assembly.LoadFrom(file);
                    }
                    catch {}
                }
            }
            try
            {
                var initTasks =
                    AppDomain.CurrentDomain.GetAssemblies().ToList()
                        .SelectMany(s => s.GetTypes())
                        .Where(t => t.IsClass && !t.IsAbstract && typeof (IContainerInitializer).IsAssignableFrom(t))
                        .ToList();

                foreach (var t in initTasks.Where(t => !t.Namespace.StartsWith("BoC."))) //first user's tasks
                {
                    ((IContainerInitializer) Activator.CreateInstance(t)).Execute();
                }
                foreach (var t in initTasks.Where(t => t.Namespace.StartsWith("BoC."))) //now ours
                {
                    ((IContainerInitializer)Activator.CreateInstance(t)).Execute();
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
    }
}
