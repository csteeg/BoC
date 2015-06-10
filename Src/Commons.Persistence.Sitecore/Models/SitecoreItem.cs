using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.ContentSearch;
using Sitecore.Globalization;

namespace BoC.Persistence.SitecoreGlass.Models
{
    [SitecoreType(true, "{66858544-a2ca-4d15-8a41-65d4313cb9c5}", Cachable = true)]
    public class SitecoreItem: BaseEntity<Guid>, ISearchable, IConvertible
    {
        [IndexField(BuiltinFields.Group)]
        [ScaffoldColumn(false)]
        override public Guid Id { get; set; }

        [IndexField(BuiltinFields.Parent)]
        [ScaffoldColumn(false)]
        virtual public Guid ParentId { get; set; }

        [ScaffoldColumn(false)]
        [IndexField(BuiltinFields.Path)]
        virtual public IEnumerable<Guid> ParentIds { get; set; }

        [ScaffoldColumn(false)]
        virtual public Guid TemplateId { get; set; }

        [IndexField(BuiltinFields.AllTemplates)]
        [SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
        [ScaffoldColumn(false)]
        virtual public IEnumerable<Guid> BaseTemplateIds { get; set; }

        [IndexField(BuiltinFields.FullPath)]
        [ScaffoldColumn(false)]
        virtual public string SitecorePath { get; set; }
        [IndexField(BuiltinFields.Name)]
        [ScaffoldColumn(false)]
        virtual public string Name { get; set; }
        [ScaffoldColumn(false)]
        virtual public string Icon { get; set; }
        [ScaffoldColumn(false)]
        virtual public IEnumerable<ISitecoreItem> Children { get; set; }
        [ScaffoldColumn(false)]
        virtual public ISitecoreItem Parent { get; set; }

        [IndexField(BuiltinFields.DisplayName)]
        [ScaffoldColumn(false)]
        virtual public string DisplayName { get; set; }

        [IndexField("__sortorder")]
        virtual public int SortOrder { get; set; }

        [IndexField(BuiltinFields.Language)]
        [SitecoreInfo(SitecoreInfoType.Language)]
        [ScaffoldColumn(false)]
        virtual public Language Language { get; set; }

        /// <summary>
        /// Only used to enable linq queries on non-strong typed fields
        /// </summary>
        /// <value>
        /// The <see cref="Object"/>.
        /// </value>
        /// <param name="fieldIndex">Index of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        [ScaffoldColumn(false)]
        virtual public Object this[String fieldIndex] { get { throw new NotSupportedException(); } }

        public override string ToString()
        {
            return String.IsNullOrEmpty(DisplayName) ? Name : DisplayName;
        }

        #region IConvertible -> because of a bug in LinqToLuceneIndex<T>.ApplyScalarMethods we need to implement this :(
        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType.IsAssignableFrom(GetType()))
            {
                return this;
            }
            return null;
        }
        #endregion
    }
}
