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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Libraries.Caches
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
