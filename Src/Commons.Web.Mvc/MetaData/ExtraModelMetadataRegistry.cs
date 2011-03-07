using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraModelMetadataRegistry : IExtraModelMetadataRegistry
    {
        private readonly Dictionary<Type, List<Attribute>> registeredAttributes = new Dictionary<Type, List<Attribute>>();
        public void Extend<TFrom,TWith>()
        {
            Extend(typeof (TFrom), typeof (TWith));
        }

        public void Extend(Type type, Type with)
        {
            //AddAttribute(type, new ExtendWithTypeAttribute(with));
            TypeDescriptor.AddProvider(new ExtraModelMetadataTypeDescriptionProvider(type), type);
            TypeDescriptor.AddAttributes(type, new ExtendWithTypeAttribute(with));
        }

        public void AddAttribute(Type onType, Attribute attribute)
        {
            if (!registeredAttributes.ContainsKey(onType))
                registeredAttributes.Add(onType, new List<Attribute>());

            registeredAttributes[onType].Add(attribute);
        }

        public IEnumerable<Attribute> GetExtraAttributes(Type onType)
        {
            if (registeredAttributes.ContainsKey(onType))
                return registeredAttributes[onType].AsReadOnly();

            return new Attribute[0];
        }
    }
}
