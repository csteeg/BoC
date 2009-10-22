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
            string binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ?? AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
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
                        .Where(t => t.IsClass && !t.IsAbstract && typeof (IContainerInitializer).IsAssignableFrom(t));

                foreach (var t in initTasks)
                {
                    ((IContainerInitializer) Activator.CreateInstance(t)).Execute();
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
