using System;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace BoC.Persistence.SitecoreGlass.Models
{
    [SitecoreType(true, "{41d82537-4720-409b-903e-2bb2f64312f2}")]
    public interface ISearchable : ISitecoreItem
    {
        Object this[String fieldIndex] { get; }
    }
}
