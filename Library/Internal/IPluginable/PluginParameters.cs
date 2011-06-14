using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace sones.Library.VersionedPluginManager
{
    public class PluginParameters<TValue> : IDictionary<string,TValue>
    {
        private Dictionary<String, TValue> internalParamDict;

        public PluginParameters()
        {
            internalParamDict = new Dictionary<string, TValue>();
        }

        public void Add(string key, TValue value)
        {
            internalParamDict.Add(key.ToUpper(), value);
        }

        public bool ContainsKey(string key)
        {
            return internalParamDict.ContainsKey(key.ToUpper());
        }

        public ICollection<string> Keys
        {
            get
            {
                return internalParamDict.Keys;
            }
        }

        public bool Remove(string key)
        {
            return internalParamDict.Remove(key.ToUpper());
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return internalParamDict.TryGetValue(key.ToUpper(), out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                return internalParamDict.Values;
            }
        }

        public TValue this[string key]
        {
            get
            {
                return internalParamDict[key.ToUpper()];
            }
            set
            {
                internalParamDict[key.ToUpper()] = value;
            }
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            internalParamDict.Add(item.Key.ToUpper(), item.Value);
        }

        public void Clear()
        {
            internalParamDict.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return internalParamDict.Contains(new KeyValuePair<string, TValue>(item.Key.ToUpper(), item.Value));
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            ((ICollection)internalParamDict).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return internalParamDict.Count();
            }            
        }

        public bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return internalParamDict.Remove(item.Key.ToUpper());
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return internalParamDict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return internalParamDict.GetEnumerator();
        }
    }
}
