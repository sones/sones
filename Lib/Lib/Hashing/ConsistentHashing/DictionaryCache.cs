#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Lib.Hashing.ConsistentHashing
{
    public class DictionaryCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _InternalDictionary;

        public DictionaryCache()
        {
            _InternalDictionary = new Dictionary<TKey, TValue>();
        }

        #region ICache Members

        public void AddItem(TKey key, TValue value)
        {
            if (!_InternalDictionary.ContainsKey(key))
            {
                _InternalDictionary.Add(key, value);
            }
        }

        public void RemoveItem(TKey key)
        {
            _InternalDictionary.Remove(key);
        }

        public TValue GetValue(TKey key)
        {
            TValue retVal = default(TValue);

            _InternalDictionary.TryGetValue(key, out retVal);

            return retVal;
        }        

        public String ItemName
        {
            get { return System.Convert.ToString(this.GetHashCode()); }
        }

        public TValue GetItem(TKey key)
        {
            return GetValue(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _InternalDictionary.GetEnumerator();
        }

        public void RemoveAllItems()
        {
            _InternalDictionary.Clear();
        }

        public Int64 Count()
        {
            return _InternalDictionary.Count();
        }

        #endregion
    }
}
