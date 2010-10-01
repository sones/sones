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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace sones.Lib.Hashing.Filter
{
    public abstract class AFilter<T>
    {

        #region private members

        protected BitArray _BitArray;

        /// <summary>
        /// number of bits in the bitarray
        /// </summary>
        protected Int32 _FilterSize;

        /// <summary>
        /// Number of rounds the hash functions is calculated.
        /// </summary>
        protected UInt32 _K;

        /// <summary>
        /// Number of elements inserted into the filter
        /// </summary>
        protected UInt32 _NumberOAddedfItems = 0;

        /// <summary>
        /// Expected Number of elements inserted into the filter
        /// </summary>
        protected UInt32 _ExpectedNumberOfAddedItems = 0;

        /// <summary>
        /// Default Character Encoding to convert strings to byte[]
        /// </summary>
        protected static Encoding _enc = Encoding.Default;

        #endregion

        #region constructors

        public AFilter(Int32 myFilterSize)
        {
            _FilterSize = myFilterSize;

            _NumberOAddedfItems = 0;

            _BitArray = new BitArray(myFilterSize, false);
        }

        #endregion

        #region fields

        /// <summary>
        /// The number of inserted items into the filter
        /// </summary>
        public UInt32 Count
        {
            get
            {
                return _NumberOAddedfItems;
            }
        }

        /// <summary>
        /// The value of K.
        /// </summary>
        public UInt32 K
        {
            get
            {
                return _K;
            }
        }


        /// <summary>
        /// p - probability for "false positive"
        /// 
        /// description:
        /// 
        ///     if the filter conjectures, that a given item is in the set,
        ///     then p is the probability that this is wrong
        /// 
        ///     The probability is depends on the size of the bitarray m and
        ///     of the value of k.
        /// 
        ///     -> p decreases when m inreases
        ///     -> p decreases when k increases
        /// </summary>
        public double FalsePositiveProbability
        {
            get
            {
                return GetFalsePositiveProbability(_NumberOAddedfItems);
            }
        }

        public double ExpectedFalsePositiveProbability
        {
            get
            {
                return GetExpectedFalsePositiveProbability();
            }
        }

        #endregion

        #region false positive probability

        /// <summary>
        /// p - probability for "false positive"
        /// 
        /// description:
        /// 
        ///     if the filter conjectures, that a given item is in the set,
        ///     then p is the probability that this is wrong
        /// 
        /// math: (taken from http://en.wikipedia.org/wiki/Bloom_filter)
        ///         
        ///     m - number of bits in the bitarray
        ///     k - number of hash functions used by the filter
        ///     n - number of expected elements in the item set
        ///     
        ///     1) probability that a particular bit is still 0
        ///     (after inserting n keys into a bitarray of m bits)
        ///     
        ///     q = (1 - 1/m)^(k * n)
        ///     
        ///     2) probability of a false positive
        ///     
        ///     p = (1 - q)^k ~ (1 - e^(-k * n/m))^k
        /// 
        ///     3) assuming that k = m/n * ln(2)
        ///     
        ///     p = 2^(-k) = (1/2)^k ~ 0.6185 * m/n
        /// 
        /// </summary>
        /// <param name="mySize"></param>
        /// <returns></returns>
        protected double GetFalsePositiveProbability(UInt32 myNumberOfItems)
        {
            //(1 - e^(-k * n/m))^k
            return Math.Pow((1 - Math.Exp(-_K * (double)myNumberOfItems / (double)_FilterSize)), _K);
        }

        protected double GetExpectedFalsePositiveProbability()
        {
            return GetFalsePositiveProbability(_ExpectedNumberOfAddedItems);
        }

        #endregion

        #region calculation of k

        /// <summary>
        /// Calculates the optimal value for k depending on the expected count of values
        /// to be added into the filter and the size of the filter.
        /// </summary>
        /// <param name="myFilterSize">number of bits in the filter</param>
        /// <param name="myExpectedNumberOfItems">number of items to add into the filter</param>
        /// <returns>The optimal value for K</returns>
        protected UInt32 GetOptimalValueForK(Int32 myFilterSize, UInt32 myExpectedNumberOfItems)
        {
            return (UInt32)Math.Round((myFilterSize / myExpectedNumberOfItems) * Math.Log(2.0));
        }

        #endregion

        #region Add

        public abstract void Add(T myItem);

        public void Add(ICollection<T> myItems)
        {
            foreach (T item in myItems)
            {
                Add(item);
            }
        }

        #endregion

        #region Contains

        public abstract bool Contains(T myItem);

        public bool Contains(ICollection<T> myItems)
        {
            foreach (T item in myItems)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Calculate Hash

        /// <summary>
        /// Calculates the have based on a given string and a seed value
        /// </summary>
        /// <param name="myItemString"></param>
        /// <param name="mySeed"></param>
        /// <returns></returns>
        protected Int32 CalculateHash(IHashFunction myHashFunction, String myItemString, uint mySeed)
        {
            //add seed to the string
            myItemString += mySeed.ToString();
            //get byte[]
            var itemBytes = _enc.GetBytes(myItemString);
            //calc hash
            var hash = myHashFunction.Hash(itemBytes, itemBytes.Length, mySeed);
            //fit it to the size of the bitarray
            hash %= _FilterSize;

            return (Int32)hash;
        }

        protected Int32 CalculateHash(IHashFunction myHashFunction, String myItemString)
        {
            return CalculateHash(myHashFunction, myItemString, 0);
        }

        #endregion
    
    }

}
