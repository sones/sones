/*
 * ConsistentHashing
 * (c) Achim Friedland, 2008 - 2009
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

#endregion

namespace sones.Lib.Hashing
{

    /// <summary>
    /// Implementation of ConsistentHashing
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>

    public class ConsistentHashing<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : IComparable
    {

        #region Data

        private readonly SortedList<TKey, Dictionary<TKey, TValue>> _ConsistentDictionaries;
        private readonly MD5                                        _MD5;

        #endregion

        #region Constructors

        public ConsistentHashing()
        {
            _ConsistentDictionaries = new SortedList<TKey, Dictionary<TKey, TValue>>();
            _MD5                    = MD5.Create();
        }

        #endregion


        #region (private) GetDictionary(myKey)

        private IDictionary<TKey, TValue> GetDictionary(TKey myKey)
        {

            var _WorkerKey              = _MD5.ComputeHash(Encoding.UTF8.GetBytes(myKey.ToString()));
            var _BestMatchingDictionary = _ConsistentDictionaries.Where(e => e.Key.CompareTo(myKey) >= 0).First().Value;

            if (_BestMatchingDictionary == null)
                return _ConsistentDictionaries.First().Value;
            
            return _BestMatchingDictionary;

        }

        #endregion


        #region AddWorker(myWorkerID, myReplicas)

        public void AddWorker(TKey myWorkerID, Int32 myReplicas)
        {

            var _NewDictionary  = new Dictionary<TKey, TValue>();
            var _WorkerKey      = _MD5.ComputeHash(Encoding.UTF8.GetBytes(myWorkerID.ToString()));

            _ConsistentDictionaries.Add(myWorkerID, _NewDictionary);

        }

        #endregion

        #region RemoveWorker(myWorkerID)

        public void RemoveWorker(TKey myWorkerID)
        {

            var _KeyValuePairs = new List<KeyValuePair<TKey,TValue>>();

            foreach (var _KeyValuePair in _ConsistentDictionaries[myWorkerID])
                _KeyValuePairs.Add(_KeyValuePair);

            _ConsistentDictionaries.Remove(myWorkerID);

            foreach (var _KeyValuePair in _KeyValuePairs)
                this.Add(_KeyValuePair);

        }

        #endregion


        #region IDictionary<TKey, TValue> members

        public void Add(TKey myKey, TValue myValue)
        {
            GetDictionary(myKey).Add(myKey, myValue);
        }

        public Boolean ContainsKey(TKey myKey)
        {
            return GetDictionary(myKey).ContainsKey(myKey);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Boolean Remove(TKey myKey)
        {
            return GetDictionary(myKey).Remove(myKey);
        }

        public Boolean TryGetValue(TKey myKey, out TValue myValue)
        {
            return GetDictionary(myKey).TryGetValue(myKey, out myValue);
        }

        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TValue this[TKey myKey]
        {

            get
            {
                return GetDictionary(myKey)[myKey];
            }

            set
            {
                GetDictionary(myKey)[myKey] = value;
            }

        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            GetDictionary(myKeyValuePair.Key).Add(myKeyValuePair);
        }

        public void Clear()
        {
            foreach (var _dictionary in _ConsistentDictionaries)
                _dictionary.Value.Clear();
        }

        public Boolean Contains(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            return GetDictionary(myKeyValuePair.Key).Contains(myKeyValuePair);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] myArray, Int32 myArrayIndex)
        {
            throw new NotImplementedException();
        }

        public Int32 Count
        {
            get
            {
                return (from _dict in _ConsistentDictionaries.Values select _dict.Count).Sum();
            }
        }

        public Boolean IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Boolean Remove(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            return GetDictionary(myKeyValuePair.Key).Remove(myKeyValuePair);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
