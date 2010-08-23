/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* SimpleCache<TKey, TValue>
 * (c) Stefan Licht 2009
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace sones.Lib.Caches
{

    /// <summary>
    /// A simple cache implementation. Primary to test the ASimpleCache
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>

    public class SimpleCache<TKey, TValue> : ASimpleCache<TKey, TValue>
        where TKey : IComparable
    {

        #region Contructor

        public SimpleCache(String myCacheName, CacheSettings myCacheSettings)
            : base(myCacheName, myCacheSettings, new Dictionary<TKey, SimpleCacheItem<TKey, TValue>>())
        { }

        #endregion

        #region Add

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects, !DependsOnCacheItem
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        public new void Add(TKey myKey, TValue myValue)
        {
            base.Add(myKey, myValue, false);
        }

        /// <summary>
        /// Add a new item to the cache. !AllowOverwritingNotPinnedObjects, !DependsOnCacheItem
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        public new void Add(TKey myKey, TValue myValue, Boolean myIsPinned)
        {
            base.Add(myKey, myValue, myIsPinned, false);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True to overwrite an already existing pinned item.</param>
        public new void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects)
        {
            base.Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, false);
        }

        /// <summary>
        /// Add a new item to the cache
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True, if you allow to overwrite cache items which are not pinned.</param>
        /// <param name="myAllowOverwritingPinnedObjects">True, if you allow to overwrite cache items which are pinned.</param>
        public new void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, Boolean myAllowOverwritingPinnedObjects)
        {
            base.Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, myAllowOverwritingPinnedObjects);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        public new void Add(TKey myKey, TValue myValue, TKey myDependsOnCacheItem)
        {
            base.Add(myKey, myValue, false, false, myDependsOnCacheItem);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myDependsOnCacheItem">The key of item, on which this new item depends on</param>
        public new void Add(TKey myKey, TValue myValue, Boolean myIsPinned, TKey myDependsOnCacheItem)
        {
            base.Add(myKey, myValue, myIsPinned, false, false, myDependsOnCacheItem);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True to overwrite an already existing pinned item.</param>
        /// <param name="myDependsOnCacheItem">The key of item, on which this new item depends on</param>
        public new void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, TKey myDependsOnCacheItem)
        {
            base.Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, false, myDependsOnCacheItem);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True to overwrite an already existing pinned item.</param>
        /// <param name="myAllowOverwritingPinnedObjects">True, if you allow to overwrite cache items which are pinned.</param>
        /// <param name="myDependsOnCacheItem">The key of item, on which this new item depends on</param>
        public new void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, Boolean myAllowOverwritingPinnedObjects, TKey myDependsOnCacheItem)
        {
            base.Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, myAllowOverwritingPinnedObjects, myDependsOnCacheItem);
        }

        #endregion

        #region Remove

        public new void Remove(TKey myKey)
        {
            base.Remove(myKey);
        }

        public new void Remove(TKey myKey, Boolean myRemovePinnedItems, Boolean myRemoveNotExpiredDependingItems)
        {
            base.Remove(myKey, myRemovePinnedItems, myRemoveNotExpiredDependingItems);
        }

        #endregion

        #region MarkForRemove

        public new void MarkForRemove(TKey myKey)
        {
            base.MarkForRemove(myKey);
        }

        #endregion

        #region Get

        public new TValue Get(TKey myKey)
        {
            return base.Get(myKey);
        }

        #endregion

        #region Contains

        public Boolean Contains(TKey myKey)
        {
            return base.ContainsKey(myKey);
        }

        #endregion

        #region GetDependingItems

        public new HashSet<TValue> GetDependingItems(TKey myKey)
        {
            return base.GetDependingItems(myKey);
        }

        #endregion
    }
}
