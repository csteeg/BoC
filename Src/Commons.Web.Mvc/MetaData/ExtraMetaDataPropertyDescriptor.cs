using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraMetaDataPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor baseDescriptor;
        private readonly Type extensionType;

        public ExtraMetaDataPropertyDescriptor(PropertyDescriptor baseDescriptor, Type extensionType): base(baseDescriptor)
        {
            this.baseDescriptor = baseDescriptor;
            this.extensionType = extensionType;
        }

        class WeakReferenceForType: WeakReference
        {
            public WeakReferenceForType(object target, Type forType) : base(target)
            {
                ForType = forType;
            }
            public Type ForType { get; private set; }
        }
        static readonly IDictionary<WeakReferenceForType, object> objectReferences = new Dictionary<WeakReferenceForType, object>();
        static readonly object objectReferences_LOCK = new object();
        object GetWrappedComponent(object component)
        {
            if (component == null)
                return null;

            lock (objectReferences_LOCK)
            {
                //cleanup on every request
                objectReferences.Where(kv => kv.Key.Target == null).Select(kv => objectReferences.Remove(kv));

                var key =
                    objectReferences.Keys.FirstOrDefault(k => 
                                                    k.Target == component 
                                                    && k.ForType == extensionType);

                object value;
                if (key == null)
                {
                    key = new WeakReferenceForType(component, extensionType);
                    value = Activator.CreateInstance(extensionType);
                    objectReferences.Add(key, value);
                }
                else
                {
                    value = objectReferences[key] ?? Activator.CreateInstance(extensionType);
                }

                return value;
            }
        }

        public override bool CanResetValue(object component)
        {
            return baseDescriptor.CanResetValue(GetWrappedComponent(component));
        }

        public override Type ComponentType
        {
            get { return baseDescriptor.ComponentType; }
        }

        public override object GetValue(object component)
        {

            return baseDescriptor.GetValue(GetWrappedComponent(component));
        }

        public override bool IsReadOnly
        {
            get { return baseDescriptor.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return baseDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            baseDescriptor.ResetValue(GetWrappedComponent(component));
        }

        public override void SetValue(object component, object value)
        {
            baseDescriptor.SetValue(GetWrappedComponent(component), value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return baseDescriptor.ShouldSerializeValue(GetWrappedComponent(component));
        }
    }
}