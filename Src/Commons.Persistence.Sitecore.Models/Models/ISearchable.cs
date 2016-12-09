using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sitecore.ContentSearch;

namespace BoC.Persistence.SitecoreGlass.Models
{
    public interface ISearchable : ISitecoreItem, ISearchResult
	{
		[IndexField(BuiltinFields.Parent)]
		Guid ParentId { get; set; }

		[ScaffoldColumn(false)]
		[IndexField(BuiltinFields.Path)]
		IEnumerable<Guid> ParentIds { get; set; }
	}
}
