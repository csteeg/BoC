using System.Linq;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    using System;

    public static class Bootstrapper
    {
        static Bootstrapper()
        {
            try
            {
                if (!IoC.IsInitialized())
                    IoC.InitializeWith(new DependencyResolverFactory().CreateInstance());
            }
            catch (ArgumentException)
            {
                // Config file is Missing
            }
        }

        public static void RegisterAllTasksAndRunThem(Func<Type, bool> where)
        {
            RegisterAllTasks(where);
            Run();
        }

        public static void RegisterAllTasks(Func<Type, bool> where)
        {
            var tasks = AppDomain.CurrentDomain.GetAssemblies().ToList()
                    .SelectMany(s => s.GetTypes());
            
            var backgroundTasks = tasks
                    .Where(t => t.IsClass && !t.IsAbstract && !t.Assembly.GetName().Name.StartsWith("System") && typeof(IBackgroundTask).IsAssignableFrom(t) && where(t));

            foreach (var type in backgroundTasks)
            {
                IoC.RegisterType(typeof(IBackgroundTask), type);
            }

            var boostraps = tasks.Where(t => t.IsClass && !t.IsAbstract && typeof(IBootstrapperTask).IsAssignableFrom(t) && where(t));
            foreach (var t in boostraps)
            {
                IoC.RegisterType(typeof(IBootstrapperTask), t);
            }

            var postbootstraps = 
                tasks.Where(t => t.IsClass && !t.IsAbstract && typeof(IPostBootstrapperTask).IsAssignableFrom(t) && where(t));
            foreach (var t in postbootstraps)
            {
                IoC.RegisterType(typeof(IPostBootstrapperTask), t);
            }
        }

        public static void Run()
        {
            var tasks = IoC.ResolveAll<IBootstrapperTask>();
            foreach (var task in tasks)
            {
                task.Execute();
            }
            
            var posttasks = IoC.ResolveAll<IPostBootstrapperTask>();
            foreach (var task in posttasks)
            {
                task.Execute();
            }
        }
    }
}