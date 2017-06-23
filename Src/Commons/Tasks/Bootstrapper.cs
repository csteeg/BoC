using System;
using System.Collections.Generic;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class Bootstrapper
    {
        private readonly IDependencyResolver dependencyResolver;
        private static object bootstrapper_lock = new object();
        public bool Executed { get; set; }

        public Bootstrapper(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public static readonly ICollection<Func<Type, bool>> TaskFilters = new List<Func<Type, bool>>() 
            { 
                type => typeof(IBootstrapperTask).IsAssignableFrom(type),
                type => !type.IsAbstract,
                type => type.IsClass
            };

        public virtual void Run()
        {
            if (Executed)
                return;
            lock (bootstrapper_lock)
            {
                if (Executed)
                    return;

                var tasks = dependencyResolver.ResolveAll<IBootstrapperTask>();
                foreach (var task in tasks)
                {
                    task.Execute();
                }
                Executed = true;
            }
        }
    }
}