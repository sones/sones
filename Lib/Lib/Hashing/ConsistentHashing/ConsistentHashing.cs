/*
 * ConsistentHashing
 * (c) Dirk Bludau, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Hashing;

#endregion

namespace Lib.Hashing.ConsistentHashing
{
    public class ConsistentHashing<T, TKey, TValue> where T : ICache<TKey, TValue>, new()
    {
        #region Data

        private IHashFunction                  _HashFunction;
        private SortedDictionary<Int64, T>     _Circle;
        
        #endregion

        #region constructors

        public ConsistentHashing(IHashFunction hashFunc)
        {
            _Circle         = new SortedDictionary<Int64, T>();
            _HashFunction   = hashFunc;
        }

        #endregion

        #region public methods

        /// <summary>
        /// add a new cache/server in the circle
        /// </summary>
        /// <param name="cache">a cache or server item</param>
        public void AddCache(T cache)
        {
            var hashValue = GetHashValue(cache.ItemName);

            lock (this)
            {
                if (_Circle.Count > 1)
                {
                    var nextCache = FindNextCache(hashValue);

                    if (nextCache.Count() > 0)
                    {
                        var iterator = nextCache.GetEnumerator();
                        iterator.Reset();

                        //copy the interesting entries from the next cache
                        while (iterator.MoveNext())
                        {
                            var itemHashValue = GetHashValue(iterator.Current.Key.ToString());

                            if (itemHashValue < hashValue)
                            {
                                cache.AddItem(iterator.Current.Key, iterator.Current.Value);
                            }
                        }

                        //delete the entries from the next cache
                        iterator = cache.GetEnumerator();
                        iterator.Reset();

                        while (iterator.MoveNext())
                        {
                            nextCache.RemoveItem(iterator.Current.Key);
                        }
                    }
                }

                _Circle.Add(hashValue, cache);
            }
        }

        /// <summary>
        /// add an cache item
        /// </summary>
        /// <param name="key">the key of the item</param>
        /// <param name="value">the item value</param>
        public void AddItem(TKey key, TValue value)
        {
            var hashValue = GetHashValue(key.ToString());

            lock (this)
            {
                var nextCache = FindNextCache(hashValue);
                nextCache.AddItem(key, value);
            }            
        }

        /// <summary>
        /// get an cached item
        /// </summary>
        /// <param name="key">the key name</param>
        /// <returns>the value of the searched item</returns>
        public TValue GetItem(TKey key)
        {
            var hashValue = GetHashValue(key.ToString());

            lock (this)
            {
                var nextCache = FindNextCache(hashValue);
                return nextCache.GetItem(key);
            }
        }

        /// <summary>
        /// remove a item form cache or server
        /// </summary>
        /// <param name="key">the key of the item to remove</param>
        public void RemoveItem(TKey key)
        {
            var hashValue = GetHashValue(key.ToString());

            lock (this)
            {
                var nextCache = FindNextCache(hashValue);
                nextCache.RemoveItem(key);
            }
        }

        /// <summary>
        /// remove a cache/server from the circle
        /// </summary>
        /// <param name="cache">the cache to remove</param>
        public void RemoveCache(T cache)
        {
            var hashValue = GetHashValue(cache.ItemName);

            lock (this)
            {
                var cacheToDelete = ((T)_Circle[hashValue]);   
                var iterator = cacheToDelete.GetEnumerator();
                iterator.Reset();

                _Circle.Remove(hashValue);
                var nextCache = FindNextCache(hashValue);

                while (iterator.MoveNext())
                {
                    nextCache.AddItem(iterator.Current.Key, iterator.Current.Value);
                }

                cacheToDelete.RemoveAllItems();
            }
        }

        /// <summary>
        /// return the entries(caches/servers) in the circle
        /// </summary>
        public Int64 Count
        { get { return _Circle.Count; } }

        /// <summary>
        /// return the entries in a specific cache
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public Int64 EntriesInCache(T cache)
        {
            var hashValue = GetHashValue(cache.ItemName);

            return ((T)_Circle[hashValue]).Count();
        }

        #endregion

        #region private helpers

        private Int64 SearchNextNeighbour(Int64 value, List<Int64> list)
        {
            Int32 pos = list.BinarySearch(value);

            if (Math.Abs(pos) - 1 >= list.Count)
            {
                pos = 0;
            }

            return pos >= 0 ? list[pos] : list[Math.Abs(pos) - 1];
        }

        /// <summary>
        /// return the next neighbour cache/server for an hashvalue
        /// </summary>
        /// <param name="hValue">the hashvalue</param>
        /// <returns>the next cache/server</returns>
        private T FindNextCache(Int64 hValue)
        {
            Int64 pos = 0;

            if (_Circle.Count > 1)
            {
                pos = SearchNextNeighbour(hValue, _Circle.Keys.ToList());
            }
            else
            {
                pos = _Circle.Keys.ElementAt(0);
            }

            return _Circle[pos];
        }        

        /// <summary>
        /// calculate a hashvalue for a key
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>the hashvalue</returns>
        private Int64 GetHashValue(String key)
        {
            return _HashFunction.Hash(Encoding.UTF8.GetBytes(key));    
        }

        #endregion
    
    }

}
