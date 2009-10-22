using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using BoC.Validation;

namespace BoC.DomainServices.NHibernate
{
    internal class PropertyDescriptorWrapper : PropertyDescriptor
    {
        // Fields
        private PropertyDescriptor _descriptor;
        private bool _isReadOnly;

        // Methods
        public PropertyDescriptorWrapper(PropertyDescriptor descriptor, Attribute[] newAttributes)
            : base(descriptor, newAttributes)
        {
            this._descriptor = descriptor;
            ReadOnlyAttribute attribute = newAttributes.OfType<ReadOnlyAttribute>().FirstOrDefault<ReadOnlyAttribute>();
            this._isReadOnly = (attribute != null) ? attribute.IsReadOnly : false;
        }

        public override void AddValueChanged(object component, EventHandler handler)
        {
            this._descriptor.AddValueChanged(component, handler);
        }

        public override bool CanResetValue(object component)
        {
            return this._descriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return this._descriptor.GetValue(component);
        }

        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            this._descriptor.RemoveValueChanged(component, handler);
        }

        public override void ResetValue(object component)
        {
            this._descriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this._descriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return this._descriptor.ShouldSerializeValue(component);
        }

        // Properties
        public override Type ComponentType
        {
            get
            {
                return this._descriptor.ComponentType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return (this._isReadOnly || this._descriptor.IsReadOnly);
            }
        }

        public override Type PropertyType
        {
            get
            {
                return this._descriptor.PropertyType;
            }
        }

        public override bool SupportsChangeEvents
        {
            get
            {
                return this._descriptor.SupportsChangeEvents;
            }
        }
    }
}