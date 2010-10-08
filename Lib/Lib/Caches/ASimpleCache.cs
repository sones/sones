/* GraphFS
 * (c) Stefan Licht 2009
 * 
 * A simple cache
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

using sones.Lib.DataStructures.Timestamp;
using sones.Lib.Serializer;
using sones.Lib;

namespace sones.Lib.Caches
{

    public class ItemRemovedEventArgs<T> : EventArgs
    {
        public T Item { get; set; }
    }

    /// <summary>
    /// Create a cache using Collection TCache, with TKey and T
    /// </summary>
    /// <typeparam Name="TCache"></typeparam>
    /// <typeparam Name="TKey"></typeparam>
    /// <typeparam Name="T"></typeparam>
    public abstract class ASimpleCache<TKey, TValue>
        where TKey : IComparable
    {

        //NLOG: temporarily commented
        //Logger //_Logger = LogManager.GetCurrentClassLogger();

        protected Action<TKey> OnRemoveExpiredItem;

        protected event EventHandler<ItemRemovedEventArgs<TValue>> OnItemRemoved;

        public ReaderWriterLockSlim Lock
        {
            get
            {
                return _Locker_CacheItems;
            }
        }

        public void Clear()
        {
            _InternalCache.Clear();
        }

        
        #region Definitions

        /// <summary>
        /// The CacheItems holds all items of the cache 
        /// </summary>
        //protected Dictionary<TKey, SimpleCacheItem<TKey, T>> CacheItems;
        //protected SortedDictionary<TKey, SimpleCacheItem<TKey, T>> CacheItems;
        protected IDictionary<TKey, SimpleCacheItem<TKey, TValue>> _InternalCache;

        /// <summary>
        /// The TimestampReferences stores all items which are not pinned and can be removed automatically
        /// </summary>
        // if you change this, change WebAdminHttpRequest as well!
        private SortedDictionary<UInt64, TKey> _TimestampReferences;

        /// <summary>
        /// The DependingObjectsReferences stores all objects which are referenced by any dependingOnItem
        /// </summary>
        protected Dictionary<TKey, HashSet<TKey>> _ItemsDependOnOtherItem;

        protected ReaderWriterLockSlim _Locker_CacheItems;

        protected CacheSettings _GlobalCacheSettings;

        private Timer _CleanCacheTimer = null;

        protected Boolean _CleanCacheTimerOrFlushEntered = false;
        protected Object _CleanCacheTimerOrFlushEnteredLock = new Object();

        protected String _CacheName;

        protected Boolean _CacheIsClosed = false;

        #endregion

        public Boolean IsEmpty
        {
            get
            {
                return !_InternalCache.Any();
            }
        }

        #region Constructors

        public ASimpleCache(String myCacheName, CacheSettings myCacheSettings, IDictionary<TKey, SimpleCacheItem<TKey, TValue>> myCacheItems)
        {

            if (myCacheSettings == null)
                throw new ArgumentNullException("myCacheSettings");

            //CacheItems              = new Dictionary<TKey, SimpleCacheItem<TKey, T>>();
            _InternalCache                = myCacheItems;
            //CacheItems              = new TDataStructure();

            _CacheName              = myCacheName;
            _GlobalCacheSettings    = myCacheSettings.Clone();
            _TimestampReferences    = new SortedDictionary<UInt64, TKey>();
            _ItemsDependOnOtherItem = new Dictionary<TKey, HashSet<TKey>>();
            _Locker_CacheItems      = new ReaderWriterLockSlim();
            _CleanCacheTimer        = new Timer(new TimerCallback(removeExpiredItems), null, _GlobalCacheSettings.TimerDueTime, _GlobalCacheSettings.TimerPeriod);

        }

        #endregion

        #region Settings

        public CacheSettings GetSettings()
        {
            return _GlobalCacheSettings;
        }

        public void ChangeSettings(CacheSettings myNewCacheSettings)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("ChangeSettings");

            if (_GlobalCacheSettings.TimerPeriod != myNewCacheSettings.TimerPeriod)
            {
                _CleanCacheTimer.Change(myNewCacheSettings.TimerDueTime, myNewCacheSettings.TimerPeriod);
            }

            _GlobalCacheSettings = myNewCacheSettings.Clone();
        }

        #endregion

        #region Add

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects, !DependsOnCacheItem
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        protected void Add(TKey myKey, TValue myValue)
        {
            Add(myKey, myValue, false);
        }

        /// <summary>
        /// Add a new item to the cache. !AllowOverwritingNotPinnedObjects, !DependsOnCacheItem
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        protected void Add(TKey myKey, TValue myValue, Boolean myIsPinned)
        {
            Add(myKey, myValue, myIsPinned, false);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True to overwrite an already existing pinned item.</param>
        protected void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects)
        {
            Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, false);
        }

        /// <summary>
        /// Add a new item to the cache
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True, if you allow to overwrite cache items which are not pinned.</param>
        /// <param name="myAllowOverwritingPinnedObjects">True, if you allow to overwrite cache items which are pinned.</param>
        protected void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, Boolean myAllowOverwritingPinnedObjects)
        {
            Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, myAllowOverwritingPinnedObjects, default(TKey), false);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        protected void Add(TKey myKey, TValue myValue, TKey myDependsOnCacheItem)
        {
            Add(myKey, myValue, false, false, myDependsOnCacheItem);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myDependsOnCacheItem">The key of item, on which this new item depends on</param>
        protected void Add(TKey myKey, TValue myValue, Boolean myIsPinned, TKey myDependsOnCacheItem)
        {
            Add(myKey, myValue, myIsPinned, false, false, myDependsOnCacheItem, true);
        }

        /// <summary>
        /// Add a new item to the cache. !IsPinned, !AllowOverwritingNotPinnedObjects
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True to overwrite an already existing pinned item.</param>
        /// <param name="myDependsOnCacheItem">The key of item, on which this new item depends on</param>
        protected void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, TKey myDependsOnCacheItem)
        {
            Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, false, myDependsOnCacheItem, true);
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
        protected void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, Boolean myAllowOverwritingPinnedObjects, TKey myDependsOnCacheItem)
        {
            Add(myKey, myValue, myIsPinned, myAllowOverwritingNotPinnedObjects, myAllowOverwritingPinnedObjects, myDependsOnCacheItem, true);
        }

        /// <summary>
        /// Add a new item to the cache
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <param name="myValue">The value</param>
        /// <param name="myIsPinned">Pinned items will NEVER removed automatically</param>
        /// <param name="myAllowOverwritingNotPinnedObjects">True, if you allow to overwrite cache items which are not pinned.</param>
        private void Add(TKey myKey, TValue myValue, Boolean myIsPinned, Boolean myAllowOverwritingNotPinnedObjects, Boolean myAllowOverwritingPinnedObjects, TKey myDependsOnCacheItem, Boolean myDependsOnCacheItemIsSet)
        {

            //Debug.WriteLine("[SimpleCache][" + _CacheName + "][Add] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count + " # " + DateTime.Now.ToString("yyyy-dd-MM.HH:mm:ss.fffffff"));
            //Logger.Trace("[" + _CacheName + "][Add] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count + " # " + DateTime.Now.ToString("yyyy-dd-MM.HH:mm:ss.fffffff"));

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("Add");

            #region Parameter checks

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            if (myValue == null)
                throw new ArgumentNullException("myValue");

            if (myDependsOnCacheItemIsSet && myDependsOnCacheItem == null)
                throw new ArgumentNullException("myDependsOnCacheItem");

            #endregion

            #region Clean Cache check

            CleanCacheConditionChecks();

            AddPrecheckForFullCache();

            #endregion

            Boolean overwrite = false;

            lock (_Locker_CacheItems)
            {

                #region Check if the key of the new item already exist in the cache

                if ((_InternalCache.ContainsKey(myKey)))
                {
                    SimpleCacheItem<TKey, TValue> cacheItem = _InternalCache[myKey];

                    if (cacheItem.CachedItem.Value.Equals(myValue))
                    {
                        throw new CacheExceptions_CacheItemAlreadyExist("At myPosition " + myKey);
                    }
                    else
                    {
                        if (!cacheItem.IsPinned && myAllowOverwritingNotPinnedObjects)
                        {
                            overwrite = true;
                        }
                        else if (cacheItem.IsPinned && myAllowOverwritingPinnedObjects)
                        {
                            overwrite = true;
                        }
                        else
                        {
                            throw new CacheExceptions_CacheItemAlreadyExistWithDifferentContent("Is pinned at myPosition " + myKey);
                        }
                    }

                }

                #endregion

                #region Create new Cache item

                SimpleCacheItem<TKey, TValue> sci = new SimpleCacheItem<TKey, TValue>();
                sci.Created = TimestampNonce.Now;
                sci.CachedItem = new KeyValuePair<TKey, TValue>(myKey, myValue);
                sci.IsPinned = myIsPinned;
                sci.ExpirationType = _GlobalCacheSettings.ExpirationType;
                sci.AbsoluteExpirationTimeSpan = _GlobalCacheSettings.AbsoluteExpirationTimeSpan;
                sci.SlidingExpirationTimeSpan = _GlobalCacheSettings.SlidingExpirationTimeSpan;

                #endregion

                #region myDependsOnCacheItemIsSet and exists

                if (myDependsOnCacheItemIsSet && _InternalCache.ContainsKey(myDependsOnCacheItem))
                {

                    #region Add the key of the new item to the depending-item-list of the item defined by myDependsOnCacheItem

                    SimpleCacheItem<TKey, TValue> dependingObject = _InternalCache[myDependsOnCacheItem];
                    if (dependingObject.ItemsDependsOnMe == null)
                        dependingObject.ItemsDependsOnMe = new HashSet<TKey>();
                    dependingObject.ItemsDependsOnMe.Add(myKey);

                    #endregion

                    #region Store a reference of the depending item, with that we always now, if a item is referenced by any other item

                    if (!_ItemsDependOnOtherItem.ContainsKey(myKey))
                        _ItemsDependOnOtherItem.Add(myKey, new HashSet<TKey>());
                    _ItemsDependOnOtherItem[myKey].Add(myDependsOnCacheItem);

                    #endregion

                    #region renew depending parent object

                    renewItem(dependingObject, 0);

                    #endregion

                }

                #endregion

                #region ELSE: item does not exist in cache!

                else if (myDependsOnCacheItemIsSet && !_GlobalCacheSettings.AllowNotExistingDependOnMeItems)
                {
                    throw new CacheExceptions_DependingCacheItemDoesNotExist(myDependsOnCacheItem.ToString());
                }

                #endregion

                #region Overwrite if overwrite is set and there is an existing _TimestampReferences

                if (overwrite && _TimestampReferences.ContainsKey(_InternalCache[myKey].ExpiresAtTimestamp))
                {
                    _TimestampReferences.Remove(_InternalCache[myKey].ExpiresAtTimestamp);
                }

                #endregion

                #region If the item and the dependOn item is not pinned, add a new _TimestampReferences (or update it if Sliding)

                if (!sci.IsPinned && !isPinnedItem(sci, 0))
                {

                    if (overwrite && _TimestampReferences.ContainsKey(_InternalCache[myKey].ExpiresAtTimestamp))
                    {
                        _TimestampReferences.Remove(_InternalCache[myKey].ExpiresAtTimestamp);
                    }

                    //Create a new one
                    if (sci.ExpirationType == ExpirationTypes.Sliding)
                        sci.ExpiresAtTimestamp = (UInt64)TimestampNonce.Now.Add(sci.SlidingExpirationTimeSpan).Ticks;
                    else
                        sci.ExpiresAtTimestamp = (UInt64)sci.Created.Add(sci.AbsoluteExpirationTimeSpan).Ticks;

                    while (_TimestampReferences.ContainsKey(sci.ExpiresAtTimestamp))
                        sci.ExpiresAtTimestamp++;

                    _TimestampReferences.Add(sci.ExpiresAtTimestamp, myKey);
                }

                #endregion

                #region Add or overwrite item

                if (overwrite)
                    _InternalCache[myKey] = sci;
                else
                    _InternalCache.Add(myKey, sci);

                #endregion

            }
        }
    

        #endregion

        #region ContainsKey

        /// <summary>
        /// Check, whether or not a item exist in the cache
        /// </summary>
        /// <param name="myKey">The key if the item</param>
        /// <returns>True, if the item exist</returns>
        protected Boolean ContainsKey(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("Contains");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            Boolean retVal = false;

            lock (_Locker_CacheItems)
            {

                retVal = _InternalCache.ContainsKey(myKey);

            }

            return retVal;

        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes an object and all his depending objects without respecting pinned and any timestamps
        /// </summary>
        /// <param name="myKey"></param>
        protected Boolean Remove(TKey myKey)
        {
            return Remove(myKey, false, false);
        }

        /// <summary>
        /// Removes an object and all his depending objects without respecting any timestamps of the direct item
        /// </summary>
        /// <param name="myKey">The key of the item to be remove.</param>
        /// <param name="myRemovePinnedItems">False to remove only not pinned items and not pinned depending items.</param>
        /// <param name="myRemoveNotExpiredDependingItems">Remove all depending items, even if they are not expired. If set to False and there are some not expired items the item with myKey wont be removed!</param>
        protected Boolean Remove(TKey myKey, Boolean myRemovePinnedItems, Boolean myRemoveNotExpiredDependingItems)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("Remove");

            //Debug.WriteLine("[ASimpleCache][" + _CacheName + "][Remove] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count);
            //Logger.Trace("[" + _CacheName + "][Remove] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count);

            Boolean retVal = false;

            #region Parameter checks

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            #endregion


            lock (_Locker_CacheItems)
            {

                #region The item exist - so we will remove it

                if (_InternalCache.ContainsKey(myKey))
                {

                    SimpleCacheItem<TKey, TValue> sciToDelete = _InternalCache[myKey];

                    #region The item is not pinned or the myRemovePinnedItems is set

                    if (!sciToDelete.IsPinned || (sciToDelete.IsPinned && myRemovePinnedItems))
                    {
                        if (dependingItemsAreRemoveable(sciToDelete, myRemovePinnedItems, myRemoveNotExpiredDependingItems, 0))
                        {
                            removeDependingItems(sciToDelete, myRemovePinnedItems, myRemoveNotExpiredDependingItems, 0);

                            #region Remove this item from all referenced DependingOnItems lists

                            if (_ItemsDependOnOtherItem.ContainsKey(sciToDelete.CachedItem.Key))
                            {

                                foreach (TKey parentKey in _ItemsDependOnOtherItem[sciToDelete.CachedItem.Key])
                                {
                                    _InternalCache[parentKey].ItemsDependsOnMe.Remove(sciToDelete.CachedItem.Key);
                                }

                                _ItemsDependOnOtherItem.Remove(sciToDelete.CachedItem.Key);

                            }

                            #endregion

                            if (_TimestampReferences.ContainsKey(sciToDelete.ExpiresAtTimestamp))
                                _TimestampReferences.Remove(sciToDelete.ExpiresAtTimestamp);

                            _InternalCache.Remove(sciToDelete.CachedItem.Key);

                            if (OnItemRemoved != null)
                            {
                                OnItemRemoved(this, new ItemRemovedEventArgs<TValue>() { Item = sciToDelete.CachedItem.Value });
                            }
                            ////_Logger.Trace("[" + _CacheName + "][Remove] myKey: " + sciToDelete.CachedItem.Key.ToString());

                            retVal = true;
                        }
                    }

                    #endregion

                }

                #endregion

            }
            
            return retVal;

        }

        #endregion

        #region MarkForRemove

        /// <summary>
        /// Mark a item for remove (unpinn the item)
        /// </summary>
        /// <param name="myKey">The key</param>
        protected void MarkForRemove(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("MarkForRemove");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            lock (_Locker_CacheItems)
            {

                SimpleCacheItem<TKey, TValue> curItem = null;

                if (!_InternalCache.TryGetValue(myKey, out curItem))
                {
                    return;
                }

                //if (!CacheItems.ContainsKey(myKey))
                //{
                //    return;
                //}

                //SimpleCacheItem<TKey, TValue> curItem = CacheItems[myKey];

                //Debug.WriteLine("[SimpleCache][MarkForRemove] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count + " pinned: " + curItem.IsPinned + " # " + DateTime.Now.ToString("yyyy-dd-MM.HH:mm:ss.fffffff"));
                //Logger.Trace("[" + _CacheName + "][MarkForRemove] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count + " pinned: " + curItem.IsPinned + " # " + DateTime.Now.ToString("yyyy-dd-MM.HH:mm:ss.fffffff"));

                curItem.IsPinned = false;

                // the item is new in the _TimestampReferences
                if (!_TimestampReferences.ContainsKey(curItem.ExpiresAtTimestamp))
                {

                    #region Create new item in _TimestampReferences

                    if (curItem.ExpirationType == ExpirationTypes.Sliding)
                        curItem.ExpiresAtTimestamp = (UInt64)TimestampNonce.Now.Add(curItem.SlidingExpirationTimeSpan).Ticks;
                    else
                        curItem.ExpiresAtTimestamp = (UInt64)curItem.Created.Add(curItem.AbsoluteExpirationTimeSpan).Ticks;

                    while (_TimestampReferences.ContainsKey(curItem.ExpiresAtTimestamp))
                        curItem.ExpiresAtTimestamp++;

                    _TimestampReferences.Add(curItem.ExpiresAtTimestamp, myKey);

                    #endregion

                }


                // The item already exist in _TimestampReferences and is sliding - renew the item
                else if (curItem.ExpirationType == ExpirationTypes.Sliding)
                {

                    #region Sliding expiration

                    _TimestampReferences.Remove(curItem.ExpiresAtTimestamp);

                    curItem.ExpiresAtTimestamp = (UInt64)TimestampNonce.Now.Add(curItem.SlidingExpirationTimeSpan).Ticks;

                    while (_TimestampReferences.ContainsKey(curItem.ExpiresAtTimestamp))
                        curItem.ExpiresAtTimestamp++;

                    _TimestampReferences.Add(curItem.ExpiresAtTimestamp, myKey);

                    #endregion

                }

            }

            return;

        }
        
        #endregion

        #region MarkAsUnRemoveable
        
        protected void MarkAsUnRemoveable(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("MarkAsUnRemoveable");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            lock (_Locker_CacheItems)
            {

                if (!_InternalCache.ContainsKey(myKey))
                {
                    return;
                }

                SimpleCacheItem<TKey, TValue> curItem = _InternalCache[myKey];

                //Debug.WriteLine("[SimpleCache][MarkForRemove] myKey: " + myKey.ToString() + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count + " pinned: " + curItem.IsPinned + " # " + DateTime.Now.ToString("yyyy-dd-MM.HH:mm:ss.fffffff"));

                curItem.IsPinned = true;
                _TimestampReferences.Remove(curItem.ExpiresAtTimestamp);
            
            }

            return;

        }

        #endregion

        #region Private helper methods !Do not use outside a WriteLock of _Locker_CacheItems!

        /// <summary>
        /// !Do not use outside a WriteLock of _Locker_CacheItems!
        /// </summary>
        /// <param name="sciToDelete"></param>
        /// <param name="myRemovePinnedItems"></param>
        /// <param name="myRemoveNotExpiredDependingItems"></param>
        /// <param name="myCurrentDepth"></param>
        /// <returns></returns>
        private Boolean dependingItemsAreRemoveable(SimpleCacheItem<TKey, TValue> sciToDelete, Boolean myRemovePinnedItems, Boolean myRemoveNotExpiredDependingItems, Int32 myCurrentDepth)
        {

            if (myCurrentDepth >= _GlobalCacheSettings.DependingItemsDepth)
                return true;

            if (sciToDelete == null)
                return true;


            if (sciToDelete.ItemsDependsOnMe != null && sciToDelete.ItemsDependsOnMe.Count > 0)
            {

                #region Check all depending items if they are expired or not - if myRemoveNotExpiredDependingItems is false

                foreach (TKey curDependingKey in sciToDelete.ItemsDependsOnMe)
                {

                    #region The depending item does no longer exist - should never happen!

                    if (!_InternalCache.ContainsKey(curDependingKey))
                    {
                        Debug.WriteLine("[SimpleCache][Remove] The dependingKey item does no longer exist!");
                        throw new Exception("Something wicked happend. The key " + curDependingKey + " does not exist in CacheItems but NOT in _TimestampReferences. This should NEVER happen!");
                    }

                    #endregion

                    #region Check the depending item if it is not pinned and expired

                    var curDependingItem = _InternalCache[curDependingKey];

                    // we found a depending object which is still not expired
                    if ((curDependingItem.IsPinned && !myRemovePinnedItems) || (curDependingItem.ExpiresAtTimestamp > TimestampNonce.Ticks && !myRemoveNotExpiredDependingItems))
                    {
                        return false;
                    }

                    #endregion

                    // check the same for all depending items of this item - as long as myCurrentDepth has reached the condition
                    if (!dependingItemsAreRemoveable(curDependingItem, myRemovePinnedItems, myRemoveNotExpiredDependingItems, myCurrentDepth + 1))
                        return false;

                }

                #endregion

            }

            return true;

        }

        /// <summary>
        /// Be carefully using this method! You must made a DependingItemsAreRemoveable check prior to delete only if allowed!
        /// !Do not use outside a WriteLock of _Locker_CacheItems!
        /// </summary>
        /// <param name="sciToDelete"></param>
        /// <param name="myRemovePinnedItems"></param>
        /// <param name="myRemoveNotExpiredDependingItems"></param>
        /// <param name="myCurrentDepth"></param>
        /// <returns></returns>
        private Boolean removeDependingItems(SimpleCacheItem<TKey, TValue> myCacheItemToRemove, Boolean myRemovePinnedItems, Boolean myRemoveNotExpiredDependingItems, Int32 myCurrentDepth)
        {

            if (myCurrentDepth >= _GlobalCacheSettings.DependingItemsDepth)
                return true;

            if (myCacheItemToRemove == null)
                return true;

            if (myCacheItemToRemove.ItemsDependsOnMe != null && myCacheItemToRemove.ItemsDependsOnMe.Count > 0)
            {

                #region Remove all depending items if the are all expired or myRemoveNotExpiredDependingItems is True

                // remove the current item because no other depending objects exist
                foreach (TKey curDependingKey in myCacheItemToRemove.ItemsDependsOnMe)
                {
                    if (!(_InternalCache.ContainsKey(curDependingKey)))
                        throw new Exception("Something wicked happend. The key " + curDependingKey + " does not exist in CacheItems but NOT in _TimestampReferences. This should NEVER happen!");

                    SimpleCacheItem<TKey, TValue> curDependingItem = _InternalCache[curDependingKey];

                    removeDependingItems(curDependingItem, myRemovePinnedItems, myRemoveNotExpiredDependingItems, myCurrentDepth + 1);

                    #region Remove this item from all referenced DependingOnItems lists

                    //_ItemsDependOnOtherItem.Remove(myCacheItemToRemove.CachedItem.Key);
                    _ItemsDependOnOtherItem.Remove(curDependingKey);

                    #endregion

                    _TimestampReferences.Remove(curDependingItem.ExpiresAtTimestamp);
                    _InternalCache.Remove(curDependingKey);
                    ////_Logger.Trace("[" + _CacheName + "][Remove] myKey: " + curDependingKey.ToString());
                }

                #endregion


            }


            return true;

        }

        private void renewItem(SimpleCacheItem<TKey, TValue> myItem, Int32 myCurrentDepth)
        {

            if (myCurrentDepth >= _GlobalCacheSettings.DependingItemsDepth)
                return;

            if (myItem == null)
                return;

            if (!myItem.IsPinned && !isPinnedItem(myItem, myCurrentDepth))
            {
                #region Sliding expiration

                // The item already exist in _TimestampReferences and is sliding - renew the item
                if (myItem.ExpirationType == ExpirationTypes.Sliding)
                {

                    UInt64 difference = ((UInt64)TimestampNonce.Now.Add(myItem.SlidingExpirationTimeSpan).Ticks - myItem.ExpiresAtTimestamp);
                    Double tolerance = (double)difference / (double)myItem.SlidingExpirationTimeSpan.Ticks;

                    if (tolerance < _GlobalCacheSettings.SlidingExpirationTolerance)
                        return;

                    //Logger.Trace("[" + _CacheName + "][renewItem] myItem: " + myItem.ToString() + " myCurrentDepth: " + myCurrentDepth + "tolerance: " + tolerance + " difference: " + new TimeSpan((long)difference).TotalMilliseconds);

                    _TimestampReferences.Remove(myItem.ExpiresAtTimestamp);

                    myItem.ExpiresAtTimestamp = (UInt64) TimestampNonce.Now.Add(myItem.SlidingExpirationTimeSpan).Ticks;

                    while (_TimestampReferences.ContainsKey(myItem.ExpiresAtTimestamp))
                        myItem.ExpiresAtTimestamp++;

                    _TimestampReferences.Add(myItem.ExpiresAtTimestamp, myItem.CachedItem.Key);

                }

                #endregion
            }
 
            #region DependingOnItems

            if (myItem.ItemsDependsOnMe != null && myItem.ItemsDependsOnMe.Count > 0)
            {

                foreach (TKey dependingOnKey in myItem.ItemsDependsOnMe)
                {

                    if (_InternalCache.ContainsKey(dependingOnKey))
                    {
                        SimpleCacheItem<TKey, TValue> dependingObject = _InternalCache[dependingOnKey];

                        renewItem(dependingObject, myCurrentDepth + 1);

                    }

                }

            }

            #endregion

        }

        private Boolean isPinnedItem(SimpleCacheItem<TKey, TValue> myItem, Int32 myCurrentDepth)
        {

            if (myCurrentDepth > _GlobalCacheSettings.DependingItemsDepth)
                return false;

            if (myItem == null)
                return false;

            if (myItem.IsPinned)
                return true;

            #region DependingObjectsReferences

            if (_ItemsDependOnOtherItem.ContainsKey(myItem.CachedItem.Key))
            {
                foreach (TKey key in _ItemsDependOnOtherItem[myItem.CachedItem.Key])
                {
                    if (isPinnedItem(_InternalCache[key], myCurrentDepth + 1))
                        return true;
                }
            }

            #endregion

            return false;

        }

        private HashSet<TValue> getDependingItems(TKey myKey, Int32 myCurrentDepth)
        {

            HashSet<TValue> retVal = new HashSet<TValue>();

            if (myCurrentDepth > _GlobalCacheSettings.DependingItemsDepth)
                return retVal;
                
            if (myKey == null)
                return retVal;

            if (!_InternalCache.ContainsKey(myKey))
                return retVal;

            if (_InternalCache[myKey].ItemsDependsOnMe == null)
                return retVal;

            foreach (TKey key in _InternalCache[myKey].ItemsDependsOnMe)
            {
                if (!_InternalCache.ContainsKey(key))
                    throw new Exception(key + " from ItemsDependsOnMe of key " + myKey + " was not found!");

                retVal.Add(_InternalCache[key].CachedItem.Value);
                retVal.AddRange(getDependingItems(key, myCurrentDepth + 1));

            }

            return retVal;

        }

        #endregion

        #region IsPinned

        public Boolean IsPinned(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("IsPinned");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            Boolean retVal = false;

            lock (_Locker_CacheItems)
            {

                if (!_InternalCache.ContainsKey(myKey))
                {
                    return retVal;
                }

                SimpleCacheItem<TKey, TValue> curItem = _InternalCache[myKey];

                retVal = isPinnedItem(curItem, 0);

            }

            return retVal;


        }

        #endregion

        #region Get

        /// <summary>
        /// Return a value for this key. If the key could not be found it returns default(T)
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <returns>A value for the key</returns>
        protected TValue Get(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("Get");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            lock (_Locker_CacheItems)
            {

                if (!_InternalCache.ContainsKey(myKey))
                {
                    return default(TValue);
                }

                SimpleCacheItem<TKey, TValue> curItem = _InternalCache[myKey];

                renewItem(curItem, 0);

                return curItem.CachedItem.Value;
            }

        }

        #endregion

        #region GetNext

        /// <summary>
        /// Return the value for the next key. If the key could not be found it returns default(T)
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <returns>A value for the key</returns>
        protected SimpleCacheItem<TKey, TValue> GetNext(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("Get");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            SimpleCacheItem<TKey, TValue> curItem = null;

            lock (_Locker_CacheItems)
            {

                if (_InternalCache.Count == 0)
                {
                    return null;
                }

                try
                {
                    if (_InternalCache.ContainsKey(myKey))
                        curItem = _InternalCache[myKey];
                    else
                        curItem = _InternalCache.First(p => (p.Key.CompareTo(myKey) >= 0)).Value;
                }
                catch
                {
                    // there is no next element!
                    return null;
                }

                renewItem(curItem, 0);

            }

            return curItem;

        }

        #endregion

        #region GetDependingItems

        /// <summary>
        /// Return a value for this key. If the key could not be found it returns default(T)
        /// </summary>
        /// <param name="myKey">The key</param>
        /// <returns>A value for the key</returns>
        protected HashSet<TValue> GetDependingItems(TKey myKey)
        {

            if (_CacheIsClosed)
                throw new CacheExceptions_CannotOperateOnClosedCache("GetDependingItems");

            if (myKey == null)
                throw new ArgumentNullException("myKey");

            HashSet<TValue> retVal;

            lock (_Locker_CacheItems)
            {

                retVal = getDependingItems(myKey, 0);

            }

            return retVal;

        }

        #endregion

        #region Close

        public void Close()
        {
            _CacheIsClosed = true;
            _CleanCacheTimer.Dispose();
        }

        #endregion

        #region Automatically removal

        private void resetTimer()
        {
            //reset Timer
            if (_CleanCacheTimer != null && _GlobalCacheSettings != null && _GlobalCacheSettings.TimerPeriod != null)
                _CleanCacheTimer.Change(_GlobalCacheSettings.TimerPeriod, _GlobalCacheSettings.TimerPeriod);

        }

        #region Expired object remove methods

        public void removeExpiredItems(Object NullObject)
        {

            if (_CacheIsClosed)
                return;

            #region Check to enter this method only once

            if (!_CleanCacheTimerOrFlushEntered)
            {
                lock (_CleanCacheTimerOrFlushEnteredLock)
                {
                    if (_CleanCacheTimerOrFlushEntered)
                        return;

                    _CleanCacheTimerOrFlushEntered = true;
                }
            }
            else
            {
                return;
            }

            #endregion


            //Debug.WriteLine("[SimpleCache][RemoveExpiredItems][" + _CacheName + "] started at " + DateTime.Now.ToString("yyyyddMM.HHmmss.fffffff") + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count);
            //Logger.Trace("[RemoveExpiredItems][" + _CacheName + "] started at " + DateTime.Now.ToString("yyyyddMM.HHmmss.fffffff") + " CacheItems: " + CacheItems.Count + " _TimestampReferences: " + _TimestampReferences.Count);
            //NLOG: temporarily commented
            ////_Logger.Trace("[{0}] removeExpiredItems was triggered. TimestampReferences before: {1}", _CacheName, _TimestampReferences.Count);

            //if (!_Locker_CacheItems.TryEnterUpgradeableReadLock(500))
            //{
            //    _CleanCacheTimerOrFlushEntered = false;
            //    return;
            //}

            lock (_Locker_CacheItems)
            {

                #region CleanUp expired cache objects

                Int32 itemsBefore = _TimestampReferences.Count;

                var CurrentTime = TimestampNonce.Ticks;

                var _break = false;
                var Curpos = 0;
                try
                {
                    while (!_break)
                    {

                        if (_TimestampReferences.Count > 0)
                        {

                            //var _enum = _TimestampReferences.GetEnumerator();

                            while (Curpos < _TimestampReferences.Count)//)_enum.MoveNext())
                            {

                                //var current = _TimestampReferences.ElementAt(Curpos);//_enum.Current.Value;
                                var current = _TimestampReferences.Skip(Curpos).First();//_enum.Current.Value;

                                if (current.Key > CurrentTime)
                                {
                                    _break = true;
                                    break;
                                }

                                //Logger.Trace("Start removing Key " + current.Key + " from Cache - Curpos: " + Curpos);

                                if (!_InternalCache.ContainsKey(current.Value))
                                    throw new Exception("Something wicked happend. The key " + current.Value + " does not exist in CacheItems but NOT in _TimestampReferences. This should NEVER happen!");

                                if (OnRemoveExpiredItem != null)
                                    OnRemoveExpiredItem(current.Value);

                                // if we could not remove this item for any reason, take the next one
                                if (Remove(current.Value))
                                    break;
                                else
                                    Curpos++;

                            }
                            if (Curpos == _TimestampReferences.Count)
                                break;

                            Curpos = 0;

                        }
                        else break;

                    }
                }
                catch (Exception)
                {
                    //NLOG: temporarily commented
                    ////_Logger.ErrorException("", ex);
                }

                #endregion

            }

            //NLOG: temporarily commented
            ////_Logger.Trace("[{0}] removeExpiredItems was triggered. TimestampReferences after: {1}", _CacheName, _TimestampReferences.Count);

            _CleanCacheTimerOrFlushEntered = false;

            //new Thread(new ThreadStart(GarbageCollection)).Start();
            ////_Logger.Trace("[{0}] GC.WaitForFullGCComplete();", _CacheName);
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            //GC.WaitForFullGCComplete();
            //GC.WaitForFullGCApproach();
            //GC.WaitForPendingFinalizers();
            ////_Logger.Trace("[{0}] completed GC.WaitForFullGCComplete();", _CacheName);
        }

        private void GarbageCollection()
        {
            GC.Collect();
        }

        #endregion

        #endregion

        #region CleanCacheCheck conditions

        /// <summary>
        /// Static Clean Cache conditions by memory etc.
        /// </summary>
        private void CleanCacheConditionChecks()
        {
            CleanCacheCheck_MemoryCondition();
            CleanCacheCheck_PercentageCondition();
        }

        private Boolean CleanCacheCheck_PercentageCondition()
        {
            UInt32 MaxNumberOfCachedItemsThreshold = (UInt32)((double)_GlobalCacheSettings.MaxNumberOfCachedItems * _GlobalCacheSettings.ForceCleanPercentage);

            if (_TimestampReferences.Count >= MaxNumberOfCachedItemsThreshold)
            {
                //NLOG: temporarily commented
                ////_Logger.Trace("[{0}] CleanCacheCheck_PercentageCondition was triggered: {1} >= {2}", _CacheName, _TimestampReferences.Count, MaxNumberOfCachedItemsThreshold);
                removeExpiredItems(null);
                return true;
            }
            return false;
        }

        private Boolean CleanCacheCheck_MemoryCondition()
        {
            var ApplicationMemory = (UInt64) Environment.WorkingSet;
            if (ApplicationMemory > _GlobalCacheSettings.MaxAmountOfMemory)
            {
                //NLOG: temporarily commented
                ////_Logger.Trace("[{0}] CleanCacheCheck_MemoryCondition was triggered: {1} >= {2}", _CacheName, ApplicationMemory, _GlobalCacheSettings.MaxAmountOfMemory);
                removeExpiredItems(null);
                return true;
            }
            return false;
        }

        #endregion

        private void AddPrecheckForFullCache()
        {

            #region Cache is full, remove some objects

            lock (_Locker_CacheItems)
            {

                if (_GlobalCacheSettings.MaxNumberOfCachedItems <= _TimestampReferences.Count)
                {

                    removeExpiredItems(null);

                    // Still _ObjectCopyCache is full?
                    if (_GlobalCacheSettings.MaxNumberOfCachedItems <= _TimestampReferences.Count)
                    {
                        TKey OldestEntryKey = _TimestampReferences.ElementAt(0).Value;
                        //Debug.WriteLine("[ASimpleCache][removeExpiredItems][" + _CacheName + "][" + DateTime.Now.ToString() + "] AddPrecheckForFullCache _ObjectCopyCache is still full remove " + OldestEntryKey);
                        Remove(OldestEntryKey, false, true);
                    }
                }

            }
            #endregion
        
        }

    }
}
