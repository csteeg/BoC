using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using BoC.Extensions;

namespace BoC.Dynamic
{
    public class NullingExpandoObject: DynamicObject, IDictionary<string, object>
    {
        private readonly Dictionary<string, object> _innerValues= new Dictionary<string, object>();
        private PropertyInfo[] _realProperties;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        public PropertyInfo[] RealProperties
        {
            get
            {
                return _realProperties ??
                    (_realProperties =
                        this.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Where(p => p.DeclaringType.IsSubclassOf(typeof(NullingExpandoObject))).ToArray());
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _innerValues.Keys;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Keys.Select(key => new KeyValuePair<string, object>(key, this[key])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _innerValues[item.Key] = item.Value;
        }

        public void Clear()
        {
            _innerValues.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if (_innerValues.Contains(item))
                return true;
            return RealProperties.Any(p => p.Name == item.Key && p.GetValue(this, null) == item.Value);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException("CopyTo is not implemented by NullingExpandObject");
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (_innerValues.ContainsKey(item.Key))
                return ((IDictionary<string, object>)_innerValues).Remove(item);
            var existing = RealProperties.FirstOrDefault(p => p.Name == item.Key);
            if (existing == null)
                return false;
            existing.SetValue(this, null, null);
            return true;
        }

        public int Count
        {
            get
            {
                return _innerValues.Count + RealProperties.Length;
            }
        }

        public bool IsReadOnly { get { return false; } }
        public bool ContainsKey(string key)
        {
            return _innerValues.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public bool Remove(string key)
        {
            if (_innerValues.ContainsKey(key))
                return _innerValues.Remove(key);
            var existing = RealProperties.FirstOrDefault(p => p.Name == key);
            if (existing == null)
                return false;
            existing.SetValue(this, null, null);
            return true;
        }

        public bool TryGetValue(string key, out object value)
        {
            value = this[key];
            return true;
        }


        public object this[string key]
        {
            get
            {
                if (_innerValues.ContainsKey(key))
                    return _innerValues[key];
                var existing = RealProperties.FirstOrDefault(p => p.Name == key);
                if (existing == null)
                {
                    return null;
                }
                return existing.GetValue(this, null);
            }
            set
            {
                var existing = RealProperties.FirstOrDefault(p => p.Name == key);
                if (existing == null)
                {
                    _innerValues[key] = value;
                }
                else
                {
                    existing.SetValue(this, value, null);
                }
            }
        }

        public ICollection<string> Keys { get { return _innerValues.Keys.Union(RealProperties.Select(p => p.Name)).ToList(); } }
        public ICollection<object> Values { get { return _innerValues.Values.Union(RealProperties.Select(p => p.GetValue(this, null))).ToList(); } }

        public void AddPropertiesOf(object source)
        {
            if (source == null)
                return;
            foreach (var prop in source.ToDictionary())
                this.Add(prop);
        }

    }
}
