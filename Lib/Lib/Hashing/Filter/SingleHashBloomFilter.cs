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
using System.Collections;

/*
 * Description (taken from http://en.wikipedia.org/wiki/Bloom_filter)
 * 
 * The Bloom filter, conceived by Burton Howard Bloom in 1970, is a space-efficient 
 * probabilistic data structure that is used to test whether an element is a member 
 * of a set. 
 * False positives are possible, but false negatives are not. Elements can be added 
 * to the set, but not removed (though this can be addressed with a counting filter). 
 * The more elements that are added to the set, the larger the probability of false 
 * positives.
 * 
 * In this implementation the value of k is used to modify the given item to calculate a new
 * hash value.
 * 
 * In the SingleHashBloomFilter k different values are added to the item and based on the "new"
 * item the hash is calculated again.
 * 
 * Complexity of Add and Contains is O(k).
 */
namespace Lib.Hashing.Filter
{
    public class SingleHashBloomFilter<T> : AFilter<T>
    {
        #region private members

        /// <summary>
        /// The Hash Function which is called on the item
        /// </summary>
        private IHashFunction _HashFunction;

        #endregion

        #region constructors

        #region BloomFilter(Int32 myFilterSize, IHashFunction myHashFunction, Int32 myExpectedNumberOfItems)

        /// <summary>
        /// Use this constructor if you know the number of values which will be added to the filter.
        /// The value of k will be calculated based on this data.
        /// </summary>
        /// <param name="myFilterSize">Number of Bits in the filter</param>
        /// <param name="myHashFunction">HashFunction used to calc the hash.</param>
        /// <param name="myExpectedNumberOfItems">Expected number of elements which will be added to the filter.</param>
        public SingleHashBloomFilter(Int32 myFilterSize, IHashFunction myHashFunction, UInt32 myExpectedNumberOfItems) : base(myFilterSize)
        {
            _K = GetOptimalValueForK(myFilterSize, myExpectedNumberOfItems);

            _ExpectedNumberOfAddedItems = myExpectedNumberOfItems;

            _HashFunction = myHashFunction;
        }

        #endregion

        #region BloomFilter(Int32 myFilterSize, Int32 myK, IHashFunction myHashFunction)

        /// <summary>
        /// Use this constructor if you know whats the best value for k.
        /// </summary>
        /// <param name="myFilterSize">Number Of Bits in the Filter</param>
        /// <param name="myK">Number of hashes to calculate and insert.</param>
        /// <param name="myHashFunction"></param>
        public SingleHashBloomFilter(Int32 myFilterSize, UInt32 myK, IHashFunction myHashFunction) : base(myFilterSize)
        {
            _K = myK;

            _HashFunction = myHashFunction;
        }

        #endregion

        #endregion

        #region Add(T myItem)

        public override void Add(T myItem)
        {
            #region data

            var itemString = myItem.ToString();

            #endregion

            #region calc hash and insert

            for (uint i = 0; i < _K; i++)
            {
                _BitArray.Set(Math.Abs(CalculateHash(_HashFunction, itemString, i)), true);
            }

            _NumberOAddedfItems++;

            #endregion
        }

        #endregion

        #region Contains(T myItem)

        public override bool Contains(T myItem)
        {
            #region data

            var itemString = myItem.ToString();

            #endregion

            #region calc hash and check if bit is set

            for (uint i = 0; i < _K; i++)
            {
                if (!_BitArray[Math.Abs(CalculateHash(_HashFunction, itemString, i))])
                {
                    return false;
                }
            }
            return true;

            #endregion
        }
        
        #endregion

        

    }
}
