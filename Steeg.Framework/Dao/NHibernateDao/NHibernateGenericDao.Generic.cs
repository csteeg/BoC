namespace Steeg.Framework.Dao.NHibernateDao
{
    using System;
    using System.Collections;

    using NHibernate;
    using NHibernate.Collection;
    using NHibernate.Expression;
    using NHibernate.Proxy;

    using Castle.Facilities.NHibernateIntegration;
    using Castle.Facilities.NHibernateIntegration.Util;

    /// <summary>
    /// Summary description for GenericDao.
    /// </summary>
    /// <remarks>
    /// Contributed by Steve Degosserie <steve.degosserie@vn.netika.com>
    /// </remarks>
    public class NHibernateGenericDao<T, Tid> : INHibernateGenericDao<T, Tid> where T: class
    {
        private readonly ISessionManager sessionManager;
        private string sessionFactoryAlias = null;

        public NHibernateGenericDao(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        protected ISessionManager SessionManager
        {
            get { return sessionManager; }
        }

        public string SessionFactoryAlias
        {
            get { return sessionFactoryAlias; }
            set { sessionFactoryAlias = value; }
        }


        #region IGenericDAO Members

        public void Lock(T instance, Steeg.Framework.Dao.LockMode lockMode)
        {
            NHibernate.LockMode nLockMode = LockMode.None;
            if (lockMode == Steeg.Framework.Dao.LockMode.Read)
                nLockMode = LockMode.Read;
            else if (lockMode == Steeg.Framework.Dao.LockMode.Update)
                nLockMode = LockMode.Upgrade;

            using (ISession session = GetSession())
            {
                session.Lock(instance, nLockMode);
            }
        }

        public Int32 CountAll()
        {
            using (ISession session = GetSession())
            {
                IEnumerator result = session.CreateQuery(String.Format("select count(*) from {0} ", typeof(T).Name)).Enumerable().GetEnumerator();
                if (result.MoveNext())
                    return (Int32)result.Current;
                else
                    return 0;
            }
        }

        public virtual T[] FindAll()
        {
            return FindAll(int.MinValue, int.MinValue);
        }

        public virtual T[] FindAll(int firstRow, int maxRows)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));

                    //if they're both 0, they probably don't want paging
                    if (!((firstRow == 0) && (maxRows == 0)))
                    {
                        if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
                        if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
                    }
                    IList result = criteria.List();
                    if (result == null || result.Count == 0) return null;

                    T[] array = new T[result.Count];
                    result.CopyTo(array, 0);

                    return array;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindAll for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual T[] FindByProperty(string propertyName, object value, int firstRow, int maxRows)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));

                    if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);

                    if (value is ICollection) //TODO: I assumed you could never match a collection with eq... if it can, we have a problem :)
                        criteria.Add(Expression.Eq(propertyName, value));
                    else
                        criteria.Add(Expression.In(propertyName, (ICollection)value));

                    IList result = criteria.List();
                    if (result == null || result.Count == 0) return null;

                    T[] array = new T[result.Count];
                    result.CopyTo(array, 0);

                    return array;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindByProperty for " + typeof(T).Name + " with property " + propertyName, ex);
                }
            }
        }

        public virtual T FindOne(string propertyName, object value)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));
                    criteria.SetMaxResults(1);
                    criteria.Add(Expression.Eq(propertyName, value));

                    return (T)criteria.UniqueResult();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindByProperty for " + typeof(T).Name + " with property " + propertyName, ex);
                }
            }
        }

        public virtual T[] FindByProperty(string propertyName, object value)
        {
            return FindByProperty(propertyName, value, int.MinValue, int.MinValue);
        }

        public virtual T FindById(Tid id)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return (T)session.Load(typeof(T), id);
                }
                catch (ObjectNotFoundException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindByPrimaryKey for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual T Create(T instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Save(instance);
                    return instance;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Create for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void Delete(T instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Delete for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void Update(T instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Update(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Update for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void DeleteAll()
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(String.Format("from {0}", typeof(T).Name));
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform DeleteAll for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual void Save(T instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.SaveOrUpdate(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Save for " + instance.GetType().Name, ex);
                }
            }
        }

        #endregion

        #region INHibernateGenericDAO Members

        public virtual T[] FindAll(ICriterion[] criterias)
        {
            return FindAll(criterias, null, int.MinValue, int.MinValue);
        }

        public virtual T[] FindAll(ICriterion[] criterias, int firstRow, int maxRows)
        {
            return FindAll(criterias, null, firstRow, maxRows);
        }

        public virtual T[] FindAll(ICriterion[] criterias, Order[] sortItems)
        {
            return FindAll(criterias, sortItems, int.MinValue, int.MinValue);
        }

        public virtual T[] FindAll(ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));

                    if (criterias != null)
                    {
                        foreach (ICriterion cond in criterias)
                            criteria.Add(cond);
                    }

                    if (sortItems != null)
                    {
                        foreach (Order order in sortItems)
                            criteria.AddOrder(order);
                    }

                    //if they're both 0, they probably don't want paging
                    if (!((firstRow == 0) && (maxRows == 0)))
                    {
                        if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
                        if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
                    }
                    IList result = criteria.List();
                    if (result == null || result.Count == 0) return null;

                    T[] array = new T[result.Count];
                    result.CopyTo(array, 0);

                    return array;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindAll for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual T FindOne(ICriterion[] criterias)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));

                    if (criterias != null)
                    {
                        foreach (ICriterion cond in criterias)
                            criteria.Add(cond);
                    }

                    return (T)criteria.UniqueResult();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindOne for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual T[] FindAllWithCustomQuery(string queryString)
        {
            return FindAllWithCustomQuery(queryString, int.MinValue, int.MinValue);
        }

        public virtual T[] FindAllWithCustomQuery(string queryString, int firstRow, int maxRows)
        {
            if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.CreateQuery(queryString);

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList result = query.List();
                    if (result == null || result.Count == 0) return null;

                    T[] array = new T[result.Count];
                    result.CopyTo(array, 0);

                    return array;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + queryString, ex);
                }
            }
        }

        public virtual T[] FindAllWithNamedQuery(string namedQuery)
        {
            return FindAllWithNamedQuery(namedQuery, int.MinValue, int.MinValue);
        }

        public virtual T[] FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows)
        {
            if (namedQuery == null || namedQuery.Length == 0) throw new ArgumentNullException("queryString");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.GetNamedQuery(namedQuery);
                    if (query == null) throw new ArgumentException("Cannot find named query", "namedQuery");

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList result = query.List();
                    if (result == null || result.Count == 0) return null;

                    T[] array = new T[result.Count];
                    result.CopyTo(array, 0);

                    return array;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for named query : " + namedQuery, ex);
                }
            }
        }

        public void InitializeLazyProperties(T instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            using (ISession session = GetSession())
            {
                foreach (object val in ReflectionUtil.GetPropertiesDictionary(instance).Values)
                {
                    if (val is INHibernateProxy || val is IPersistentCollection)
                    {
                        if (!NHibernateUtil.IsInitialized(val))
                        {
                            session.Lock(instance, LockMode.None);
                            NHibernateUtil.Initialize(val);
                        }
                    }
                }
            }
        }

        public void InitializeLazyProperty(T instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null || propertyName.Length == 0) throw new ArgumentNullException("collectionPropertyName");

            IDictionary properties = ReflectionUtil.GetPropertiesDictionary(instance);
            if (!properties.Contains(propertyName))
                throw new ArgumentOutOfRangeException("collectionPropertyName", "Property "
                    + propertyName + " doest not exist for type "
                    + instance.GetType().ToString() + ".");

            using (ISession session = GetSession())
            {
                object val = properties[propertyName];

                if (val is INHibernateProxy || val is IPersistentCollection)
                {
                    if (!NHibernateUtil.IsInitialized(val))
                    {
                        session.Lock(instance, LockMode.None);
                        NHibernateUtil.Initialize(val);
                    }
                }
            }
        }

        #endregion

        #region Private methods
        protected ISession GetSession()
        {
            if (sessionFactoryAlias == null || sessionFactoryAlias.Length == 0)
                return sessionManager.OpenSession();
            else
                return sessionManager.OpenSession(sessionFactoryAlias);
        }
        #endregion
    }
}
