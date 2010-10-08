using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Caches
{

    public class SimpleCacheItem<TKey, TValue>
    {
        #region Data
        public KeyValuePair<TKey ,TValue> CachedItem;
        public Boolean IsPinned;
        public ExpirationTypes ExpirationType;
        #endregion

        #region constructor
        public SimpleCacheItem()
        {
        }
        #endregion

        /// <summary>
        /// Stores all CacheItems keys which depends on this item. So, if the current item needs to be deleted than all depeding object should be removed as well.
        /// Also, if an entry is changeing or added, the depending object should change the renew timestamps as well
        /// </summary>
        public HashSet<TKey> ItemsDependsOnMe;

        public DateTime Created;
        public UInt64 ExpiresAtTimestamp;
        public TimeSpan SlidingExpirationTimeSpan;
        public TimeSpan AbsoluteExpirationTimeSpan;
        public Type TypeOfItem;

        public override string ToString()
        {
            return "[" + CachedItem.Key.ToString() + "] : [" + CachedItem.Value.ToString() + "]";
        }

    }
}
