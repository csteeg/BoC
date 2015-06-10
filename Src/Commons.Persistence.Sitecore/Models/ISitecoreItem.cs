using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.ContentSearch;

namespace BoC.Persistence.SitecoreGlass.Models
{
    [SitecoreType(true, "{96a7aea8-ac67-4345-811b-a9d02f03cb05}")]
    public interface ISitecoreItem : IBaseEntity<Guid>
    {
        [SitecoreId]
        new Guid Id { get; set; }

        [SitecoreInfo(SitecoreInfoType.TemplateId)]
        Guid TemplateId { get; set; }

        [SitecoreInfo(SitecoreInfoType.FullPath)]
        string SitecorePath { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        string Name { get; set; }

        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        string DisplayName { get; set; }

        [SitecoreField("{06D5295C-ED2F-4A54-9BF2-26228D113318}")]
        string Icon { get; set; }

        [SitecoreChildren(InferType = true, IsLazy = true)]
        IEnumerable<ISitecoreItem> Children { get; set; }

        [IndexField(BuiltinFields.Parent)]
        Guid ParentId { get; set; }

        [SitecoreParent(InferType = true, IsLazy = true)]
        ISitecoreItem Parent { get; set; }

        [SitecoreField("{BA3F86A2-4A1C-4D78-B63D-91C2779C1B5E}", SitecoreFieldType.Integer, "Data", false)]
        int SortOrder { get; set; }

        [IndexField(BuiltinFields.AllTemplates)]
        [SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
        IEnumerable<Guid> BaseTemplateIds { get; set; }
    }
}
