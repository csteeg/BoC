using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoC.Persistence;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;

namespace Commons.Persistence.db4o.AutoIncrement
{
    public class AutoIncrementSupport
    {
        private readonly IdGenerator generator = new IdGenerator();
        private readonly IDictionary<Type, FieldAccess> fieldAccessors = new Dictionary<Type, FieldAccess>();

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
            var accessor = AccessorFor(obj);
            accessor.WriteValue(obj, generator, objectContainer);
        }

        private void StoreState(IObjectContainer container)
        {
            generator.StoreState(container);
        }

        private FieldAccess AccessorFor(Object obj)
        {
            var theType = obj.GetType();
            FieldAccess field = null;
            if (!fieldAccessors.TryGetValue(theType, out field))
            {
                field = FindFieldAccessor(theType);
                fieldAccessors.Add(theType, field);
            }
            return field;
        }

        private static FieldAccess FindFieldAccessor(Type theType)
        {
            var field = FindAutoIdAccessor(theType);
            return FieldAccess.Create(field);
        }

        private static MemberInfo idMemberInfo;
        private static MemberInfo FindAutoIdAccessor(Type theType)
        {
            if(typeof(IBaseEntity).IsAssignableFrom(theType))
            {
                if(idMemberInfo == null)
                {
                    idMemberInfo = theType.GetProperty(
                        "Id", 
                        BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                }
                return idMemberInfo;
            }
            else
            {
                var fields = theType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var properties =
                    theType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var accessMembers = fields.Cast<MemberInfo>().Concat(properties);
                var access = (from fieldInfo in accessMembers
                              where HasMarkerAttribute(fieldInfo)
                              select fieldInfo).FirstOrDefault();
                if (null != access)
                {
                    return access;
                }
                else if (!theType.BaseType.Equals(typeof (object)))
                {
                    return FindAutoIdAccessor(theType.BaseType);
                }
                else
                {
                    return null;
                }
            }
        }

        private static bool HasMarkerAttribute(MemberInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes(typeof(AutoIncrementAttribute), false).Length > 0;
        }

        private abstract class FieldAccess
        {
            static readonly FieldAccess NoIdFieldAvailable = new NullFieldAccess();

            public abstract void WriteValue(Object onObject, IdGenerator generator, IObjectContainer container);

            public static FieldAccess Create(MemberInfo member)
            {
                return null == member ? NoIdFieldAvailable : CreateAccessor(member);
            }

            private static RealFieldAccess CreateAccessor(MemberInfo member)
            {
                Action<object, object> access = TryAsField(member);
                access = access ?? TryAsProperty(member);
                return new RealFieldAccess(access, member.DeclaringType);
            }

            private static Action<object, object> TryAsField(MemberInfo member)
            {
                var field = member as FieldInfo;
                if (null != field)
                {
                    return (obj, id) => field.SetValue(obj, id);
                }
                return null;
            }

            private static Action<object, object> TryAsProperty(MemberInfo member)
            {
                var field = member as PropertyInfo;
                if (null != field)
                {
                    return (obj, id) => field.SetValue(obj, id, null);
                }
                return null;
            }

            private class NullFieldAccess : FieldAccess
            {
                public override void WriteValue(Object obj, IdGenerator generator, IObjectContainer container)
                {
                }
            }

            private class RealFieldAccess : FieldAccess
            {
                private readonly Action<object, object> memberAccess;
                private readonly Type forType;

                public RealFieldAccess(Action<object, object> memberAccess, Type onType)
                {
                    this.memberAccess = memberAccess;
                    this.forType = onType;
                }

                public override void WriteValue(Object obj, IdGenerator generator, IObjectContainer container)
                {
                    var id = generator.NextId(forType, container);
                    memberAccess(obj, id);
                }
            }
        }
    }
}
