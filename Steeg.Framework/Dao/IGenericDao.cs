using System;
using System.Collections.Generic;
using System.Text;

namespace Steeg.Framework.Dao
{
    public enum LockMode
    {
        None,
        Read,
        Update
    }

    public interface IGenericDao<T, Tid>
    {
        T[] FindAll();

        T[] FindAll(Int32 firstRow, Int32 maxRows);

        Int32 CountAll();

        T FindById(Tid id);

        T[] FindByProperty(String propertyName, Object value);

        T FindOne(String propertyName, Object value);

        T Create(T instance);

        void Lock(T instance, LockMode mode);

        void Update(T instance);

        void Delete(T instance);

        void DeleteAll();

        void Save(T instance);
    }
}
