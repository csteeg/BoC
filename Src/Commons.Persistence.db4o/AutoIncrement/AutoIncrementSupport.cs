using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoC.Persistence;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;

namespace BoC.Persistence.db4o.AutoIncrement
{
    public class AutoIncrementSupport
    {
        private readonly IdGenerator generator = new IdGenerator();

        private AutoIncrementSupport()
        {
        }

        public static void Install(IObjectContainer installOn)
        {
            var events = EventRegistryFactory.ForObjectContainer(installOn);
            var support = new AutoIncrementSupport();

            events.Creating += (sender, args)
                    => support.IncrementIdsFor(args.Object, installOn);
            events.Committing += (sender, args)
                    => support.StoreState(installOn);
        }

        private void IncrementIdsFor(Object obj, IObjectContainer objectContainer)
        {
            var entity = obj as IBaseEntity;
            if (entity != null)
            {
                entity.Id = generator.NextId(obj.GetType(), objectContainer);
            }
        }

        private void StoreState(IObjectContainer container)
        {
            generator.StoreState(container);
        }
        
    }
}
