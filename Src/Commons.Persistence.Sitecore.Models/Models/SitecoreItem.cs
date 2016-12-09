using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Globalization;

namespace BoC.Persistence.SitecoreGlass.Models
{
	public class SitecoreItem : BaseEntity<Guid>, ISearchable, IConvertible
	{
		[IndexField(BuiltinFields.Group)]
		[ScaffoldColumn(false)]
		public override Guid Id { get; set; }

		[IndexField(BuiltinFields.Parent)]
		[ScaffoldColumn(false)]
		Guid ISearchable.ParentId { get; set; }

		[ScaffoldColumn(false)]
		[IndexField(BuiltinFields.Path)]
		IEnumerable<Guid> ISearchable.ParentIds { get; set; }

		[ScaffoldColumn(false)]
		[IndexField(BuiltinFields.Template)]
		public virtual Guid TemplateId { get; set; }

		[IndexField(BuiltinFields.AllTemplates)]
		[ScaffoldColumn(false)]
		public virtual IEnumerable<Guid> BaseTemplateIds { get; set; }

		[IndexField(BuiltinFields.FullPath)]
		[ScaffoldColumn(false)]
		public virtual string SitecorePath { get; set; }
		[IndexField(BuiltinFields.Name)]
		[ScaffoldColumn(false)]
		public virtual string Name { get; set; }
		[ScaffoldColumn(false)]
		[IndexField(BuiltinFields.Icon)]
		public virtual string Icon { get; set; }
		[ScaffoldColumn(false)]
		public virtual IEnumerable<ISitecoreItem> Children { get; set; }
		[ScaffoldColumn(false)]
		public virtual ISitecoreItem Parent { get; set; }

		[IndexField(BuiltinFields.DisplayName)]
		[ScaffoldColumn(false)]
		public virtual string DisplayName { get; set; }

		[IndexField("__sortorder")]
		public virtual int SortOrder { get; set; }

		[IndexField(BuiltinFields.Language)]
		[ScaffoldColumn(false)]
		public virtual Language Language { get; set; }

		[IndexField("_datasource")]
		string ISearchResult.Datasource { get; set; }

		[IndexField("_content")]
		string ISearchResult.Content { get; set; }

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
		string IObjectIndexers.this[string fieldIndex]
		{
			get { throw new NotSupportedException(); }
			set { throw new NotImplementedException(); }
		}

		[ScaffoldColumn(false)]
		object IObjectIndexers.this[ObjectIndexerKey key]
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

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
