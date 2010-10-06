using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExpress.Xpo.Exceptions
{
    public class TypeNotFoundException : Exception { }
    public class DuplicateKeyPropertyException : Exception { }
}
namespace DevExpress.Xpo
{
    public class Session : IDisposable
    {
        public T GetObjectByKey<T>(object id)
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }

        public void Delete(object target)
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }

        public bool InTransaction
        {
            get { throw new NotImplementedException("Please replace me with the official XPO dll"); }
            set { throw new NotImplementedException("Please replace me with the official XPO dll"); }
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }

        public void Dispose()
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }

        public void Save(object target)
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }
    }

    public class XPQuery<T> : IQueryable<T>
    {
        public XPQuery(Session session)
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException("Please replace me with the official XPO dll");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression
        {
            get { throw new NotImplementedException("Please replace me with the official XPO dll"); }
        }

        public Type ElementType
        {
            get { throw new NotImplementedException("Please replace me with the official XPO dll"); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotImplementedException("Please replace me with the official XPO dll"); }
        }
    }
}
