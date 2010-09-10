/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* GraphFS
 * (c) Stefan Licht 2009
 * 
 * This is a special kind of a Simplecache which chaches a bunch of bytes for a position
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using sones.Lib.DataStructures.Big;
using sones.Lib.Caches;

namespace sones.StorageEngines.Caches
{
    /// <summary>
    /// This is a special kind of a Simplecache which chaches a bunch of bytes for a position
    /// </summary>
    public class ByteCache : ASimpleCache<UInt64, Byte[]>
    {

        #region Constructors

        public ByteCache(String myCacheName)
            : base(myCacheName, new CacheSettings(), new SortedDictionary<UInt64, SimpleCacheItem<UInt64, Byte[]>>())
        {
            _GlobalCacheSettings.ExpirationType = ExpirationTypes.Sliding;
        }

        public ByteCache(String myCacheName, CacheSettings myObjectCacheSettings)
            : base(myCacheName, myObjectCacheSettings, new SortedDictionary<UInt64, SimpleCacheItem<UInt64, Byte[]>>())
        {

        }

        #endregion

        #region Flushed

        /// <summary>
        /// Is invoked if the bytes were flushed on a hard disk. This will remove the pinned 
        /// flag from the item, to clean it up at some time
        /// </summary>
        /// <param name="myPosition"></param>
        public void Flushed(UInt64 myPosition)
        {
            //Debug.WriteLine("[ByteCache][WasFlushed] myPosition: " + myPosition);

            MarkForRemove(myPosition);
            //Remove(myPosition);
        }

        #endregion

        #region Cache

        /// <summary>
        /// Add an element
        /// </summary>
        /// <param name="myPosition"></param>
        /// <param name="myBytes"></param>
        public void Cache(UInt64 myPosition, Byte[] myBytes)
        {
            //Debug.WriteLine("[ByteCache][Cache] myPosition: " + myPosition + " length: " + myBytes.Length);
            Add(myPosition, myBytes, true, true, true);
        }

        #endregion

        #region Get

        public new Byte[] Get(UInt64 myPosition)
        {
            //Debug.WriteLine("[ByteCache][Get] " + myPosition + " length " + ((base.Get(myPosition) != null)?base.Get(myPosition).Length:-1));
            return base.Get(myPosition);
        }

        public Byte[] Get(UInt64 myPosition, UInt64 myLength, Dictionary<UInt64, UInt64> myMissingCachePositions)
        {
            //Debug.WriteLine("[ByteCache][Get] " + myPosition + " length (myLength) " + ((base.Get(myPosition) != null) ? base.Get(myPosition).Length : -1));
            
            Byte[] foundBytes = new Byte[myLength];
            Int64 curPos = 0;

            while (curPos < (Int64)myLength)
            {
                SimpleCacheItem<UInt64, Byte[]> nextCacheItem = base.GetNext(myPosition + (UInt64)curPos);
                if (nextCacheItem == null)
                {
                    myMissingCachePositions.Add(myPosition + (UInt64)curPos, myLength - (UInt64)curPos);
                    return foundBytes;
                }

                // check whether the next cacheitem is in the requested byte-part.
                // the key must between the start position + length AND the length must before start position + length
                if (nextCacheItem.CachedItem.Key < myPosition + myLength)
                {
                    // if the key of next cacheitem is greater than the current found bytes, we have a missing part
                    if (nextCacheItem.CachedItem.Key > myPosition + (UInt64)curPos)
                    {
                        myMissingCachePositions.Add(myPosition + (UInt64)curPos, nextCacheItem.CachedItem.Key - (myPosition + (UInt64)curPos));
                        curPos += (Int64)nextCacheItem.CachedItem.Key - ((Int64)myPosition + curPos);
                    }

                    if (nextCacheItem.CachedItem.Key + (UInt64)nextCacheItem.CachedItem.Value.Length < myPosition + myLength)
                    {
                        // the nextCacheItem fit into the missing bytes at all
                        Array.Copy(nextCacheItem.CachedItem.Value, 0, foundBytes, curPos, (Int64)nextCacheItem.CachedItem.Value.Length);
                        curPos += nextCacheItem.CachedItem.Value.Length;
                    }
                    else
                    {
                        // the nextCacheItem fit into the missing bytes but only a part of it
                        Array.Copy(nextCacheItem.CachedItem.Value, 0, foundBytes, curPos, ((Int64)myLength - curPos));
                        curPos += (Int64)myLength - curPos;
                    }
                }
                else
                {
                    // the next cahce item does not fit into the missing bytes, so add a missingCachePosition:
                    // if the next item is
                    myMissingCachePositions.Add(myPosition + (UInt64)curPos, (myLength - (UInt64)curPos));
                    curPos += (Int64)myLength - curPos;
                }
            }
            return foundBytes;
        }

        #endregion

        #region Contains

        public new Boolean Contains(UInt64 myKey)
        {
            return base.ContainsKey(myKey);
        }

        #endregion

    }
}
