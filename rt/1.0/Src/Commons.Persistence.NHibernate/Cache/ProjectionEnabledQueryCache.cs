using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Type;

namespace BoC.Persistence.NHibernate.Cache
{
    /// <summary>
    /// This class works around the bug that you can't request Projections
    /// when the query is set to cacheable...
    /// Should be fixed in the next nhibernate I hope, and then this class can be removed
    /// </summary>
    public class ProjectionEnabledQueryCache : StandardQueryCache, IQueryCache
    {
        public ProjectionEnabledQueryCache(Settings settings, IDictionary<string, string> props, UpdateTimestampsCache updateTimestampsCache, string regionName) : base(settings, props, updateTimestampsCache, regionName) {}


        bool IQueryCache.Put(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session)
        {
            //if the returntypes contains simple values, assume it's a projection:
            if (returnTypes.OfType<IType>().Any(t => t.ReturnedClass != null && (t.ReturnedClass.IsValueType || t.ReturnedClass.IsPrimitive || t.ReturnedClass == typeof(String))))
                return false;

            return this.Put(key, returnTypes, result, isNaturalKeyLookup, session);
        }
    }

    public class ProjectionEnabledQueryCacheFactory : IQueryCacheFactory
    {
        public IQueryCache GetQueryCache(string regionName,
                                                                         UpdateTimestampsCache updateTimestampsCache,
                                                                         Settings settings,
                                                                         IDictionary<string, string> props)
        {
            return new ProjectionEnabledQueryCache(settings, props, updateTimestampsCache, regionName);
        }
    }
}
