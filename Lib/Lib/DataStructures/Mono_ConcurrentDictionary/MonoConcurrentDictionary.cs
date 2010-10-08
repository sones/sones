#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

#endregion

namespace sones.Lib.DataStructures.ConcurrentDictionary_Mono
{
    public class MonoConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        #region Data

        private Dictionary<TKey, TValue> _Container;

        private static object _Logger = new object();
        
        #endregion

        #region constructors

        public MonoConcurrentDictionary()
        {
            lock (_Logger)
            {
                _Container = new Dictionary<TKey, TValue>();
            }
        }

        public MonoConcurrentDictionary(IDictionary<TKey, TValue> collection)
        {
            lock(_Logger)
            {
                _Container = new Dictionary<TKey, TValue>(collection);
            }
        }

        public MonoConcurrentDictionary(IDictionary<TKey, TValue> collection, IEqualityComparer<TKey> comparer)
        {
            lock (_Logger)
            {
                _Container = new Dictionary<TKey, TValue>(collection, comparer);
            }
        }

        #endregion

        #region concurrent Members

        public TValue AddOrUpdate(TKey key, TValue value, Func<TKey, TValue, TValue> function)
        {
            lock(_Logger)
            {
                if (!ContainsKey(key))
                {
                    Add(key, value);
                    return value;
                }
                else
                {   
                    var retVal   = function(key, value);

                    _Container[key] = retVal;

                    return value; 
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_Logger)
            {
                if (_Container.ContainsKey(key))
                {
                    value = _Container[key];
                    return true;
                }
            }
            
            value = default(TValue);
            return false;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            lock (_Logger)
            {
                if (_Container.ContainsKey(key))
                {
                    value = _Container[key];
                    return _Container.Remove(key);
                }
                else
                {
                    value = default(TValue);
                    return false;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            lock (_Logger)
            {
                return _Container.GetEnumerator();
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get 
            {
                lock (_Logger)
                {
                    return _Container.Count;
                }
            }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDictionary Members

        public void Add(object key, object value)
        {
            lock (_Logger)
            {
                try
                {
                    _Container.Add((TKey)key, (TValue)value);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Clear()
        {
            lock (_Logger)
            {
                _Container.Clear();
            }
        }

        public bool Contains(object key)
        {
            lock (_Logger)
            {
                if (key is TKey)
                    return _Container.ContainsKey((TKey)key);
                else
                    return false;
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            lock (_Logger)
            {
                return _Container.GetEnumerator();
            }
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection Keys
        {
            get 
            {
                lock (_Logger)
                {
                    return _Container.Keys;
                }
            }
        }

        public void Remove(object key)
        {
            lock (_Logger)
            {
                if(key is TKey)
                    _Container.Remove((TKey)key);
            }
        }

        public ICollection Values
        {
            get 
            {
                lock (_Logger)
                {
                    return _Container.Values;
                }
            }
        }

        public object this[object key]
        {
            get
            {
                lock (_Logger)
                {
                    try
                    {
                        return _Container[(TKey)key];
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            set
            {
                try
                {
                    _Container[(TKey)key] = (TValue)value;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        #endregion


        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            lock (_Logger)
            {
                try
                {
                    _Container.Add(key, value);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_Logger)
            {
                return _Container.ContainsKey(key);
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get 
            {
                lock (_Logger)
                {
                    return _Container.Keys;
                }
            }
        }

        public bool Remove(TKey key)
        {
            lock (_Logger)
            {
                try
                {
                    return _Container.Remove(key);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }        

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get 
            {
                lock (_Logger)
                {
                    return _Container.Values;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_Logger)
                {
                    try
                    {
                        return _Container[key];
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            set
            {
                lock (_Logger)
                {
                    try
                    {
                        _Container[key] = value;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
