/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.Library.NewFastSerializer
{

    #region UniqueStringList Nested Class
    /// <summary>
    /// Provides a faster way to store string tokens both maintaining the order that they were added and
    /// providing a fast lookup.
    /// 
    /// Based on code developed by ewbi at http://ewbi.blogs.com/develops/2006/10/uniquestringlis.html
    /// </summary>
    internal sealed class UniqueStringList
    {
        #region Static
        private const float LoadFactor = .72f;

        // Based on Golden Primes (as far as possible from nearest two powers of two)
        // at http://planetmath.org/encyclopedia/GoodHashTablePrimes.html
        private static readonly int[] primeNumberList = new int[]
				{
					// 193, 769, 3079, 12289, 49157 removed to allow quadrupling of bucket table size
					// for smaller size then reverting to doubling
					389, 1543, 6151, 24593, 98317, 196613, 393241, 786433, 1572869, 3145739, 6291469,
					12582917, 25165843, 50331653, 100663319, 201326611, 402653189, 805306457, 1610612741
				};
        #endregion Static

        #region Fields
        private string[] stringList;
        private int[] buckets;
        private int bucketListCapacity;
        private int stringListIndex;
        private int loadLimit;
        private int primeNumberListIndex;
        #endregion Fields

        #region Constructors
        public UniqueStringList()
        {
            bucketListCapacity = primeNumberList[primeNumberListIndex++];
            stringList = new string[bucketListCapacity];
            buckets = new int[bucketListCapacity];
            loadLimit = (int)(bucketListCapacity * LoadFactor);
        }
        #endregion Constructors

        #region Properties
        public string this[int index]
        {
            get { return stringList[index]; }
        }

        public int Count
        {
            get { return stringListIndex; }
        }
        #endregion Properties

        #region Methods
        public int Add(String value)
        {
            int bucketIndex = getBucketIndex(value);
            int index = buckets[bucketIndex];
            if (index == 0)
            {
                stringList[stringListIndex++] = value;
                buckets[bucketIndex] = stringListIndex;
                if (stringListIndex > loadLimit) expand();
                return stringListIndex - 1;
            }
            return index - 1;
        }
        #endregion Methods

        #region Private Methods
        private void expand()
        {
            bucketListCapacity = primeNumberList[primeNumberListIndex++];
            buckets = new int[bucketListCapacity];
            string[] newStringlist = new string[bucketListCapacity];
            stringList.CopyTo(newStringlist, 0);
            stringList = newStringlist;
            reindex();
        }

        private void reindex()
        {
            loadLimit = (int)(bucketListCapacity * LoadFactor);
            for (int stringIndex = 0; stringIndex < stringListIndex; stringIndex++)
            {
                int index = getBucketIndex(stringList[stringIndex]);
                buckets[index] = stringIndex + 1;
            }
        }

        private int getBucketIndex(String value)
        {
            int hashCode = value.GetHashCode() & 0x7fffffff;
            int bucketIndex = hashCode % bucketListCapacity;
            int increment = (bucketIndex > 1) ? bucketIndex : 1;
            int i = bucketListCapacity;
            while (0 < i--)
            {
                int stringIndex = buckets[bucketIndex];
                if (stringIndex == 0) return bucketIndex;
                if (String.CompareOrdinal(value, stringList[stringIndex - 1]) == 0) return bucketIndex;
                bucketIndex = (bucketIndex + increment) % bucketListCapacity; // Probe.
            }
            throw new InvalidOperationException("Failed to locate a bucket.");
        }
        #endregion Private Methods
    }
    #endregion UniqueStringList Nested Class

}
