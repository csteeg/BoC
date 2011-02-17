using System;
using System.Collections.Generic;

namespace BoC.Web.Mvc.MetaData
{
    public interface IExtraModelMetadataRegistry
    {
        void Extend<TFrom,TWith>();
        void Extend(Type type, Type with);
        void AddAttribute(Type onType, Attribute attribute);
        IEnumerable<Attribute> GetExtraAttributes(Type onType);
    }
}