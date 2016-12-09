using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sitecore.ContentSearch;
using Sitecore.Globalization;

namespace BoC.Persistence.SitecoreGlass.Models
{
	public interface ISitecoreItem : IBaseEntity<Guid>
	{
		[IndexField(BuiltinFields.Group)]
		new Guid Id { get; set; }

		[IndexField(BuiltinFields.Template)]
		Guid TemplateId { get; set; }

		[IndexField(BuiltinFields.FullPath)]
		string SitecorePath { get; set; }

		[IndexField(BuiltinFields.Name)]
		string Name { get; set; }

		[IndexField(BuiltinFields.DisplayName)]
		string DisplayName { get; set; }

		[IndexField(BuiltinFields.Icon)]
		string Icon { get; set; }

		IEnumerable<ISitecoreItem> Children { get; set; }

		ISitecoreItem Parent { get; set; }

		[IndexField("__sortorder")]
		int SortOrder { get; set; }

		[IndexField(BuiltinFields.AllTemplates)]
		IEnumerable<Guid> BaseTemplateIds { get; set; }

		[IndexField(BuiltinFields.Language)]
		Language Language { get; set; }
	}
}
