
namespace Steeg.Framework.Dao.NHibernateDao
{
    using System;
    using NHibernate.Expression;

    /// <summary>
    /// Summary description for INHibernateGenericDao.
    /// </summary>
    /// <remarks>
    /// Contributed by Steve Degosserie <steve.degosserie@vn.netika.com>
    /// </remarks>
    public interface INHibernateGenericDao<T, Tid> : Steeg.Framework.Dao.IGenericDao<T, Tid> where T: class
    {
        T FindOne(ICriterion[] criterias);
        
        T[] FindAll(ICriterion[] criterias);

        T[] FindAll(ICriterion[] criterias, int firstRow, int maxRows);

        T[] FindAll(ICriterion[] criterias, Order[] sortItems);

        T[] FindAll(ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows);

        T[] FindAllWithCustomQuery(string queryString);

        T[] FindAllWithCustomQuery(string queryString, int firstRow, int maxRows);

        T[] FindAllWithNamedQuery(string namedQuery);

        T[] FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows);

        void InitializeLazyProperties(T instance);

        void InitializeLazyProperty(T instance, string propertyName);
    }
}
