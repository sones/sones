using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
 * In this implementation the value of k is the number of used HashFunctions.
 * 
 * In the MultiHashBloomFilter k different hash values are calculated using k different
 * hash functions.
 * 
 * Complexity of Add and Contains is O(k).
 */
namespace sones.Lib.Hashing.Filter
{
    public class MultiHashBloomFilter<T> : AFilter<T>
    {
        #region private members

        private LinkedList<IHashFunction> _hashFunctions;

        #endregion

        #region constructors

        public MultiHashBloomFilter(Int32 myExpectedFilterSize, LinkedList<IHashFunction> myHashFunctions) : base(myExpectedFilterSize)
        {
            if (myHashFunctions.Count == 0)
            {
                throw new ArgumentException("there is at least one hash function needed");
            }
            //remove duplicates
            _hashFunctions = myHashFunctions;

            _K = (uint)_hashFunctions.Count;
        }

        #endregion

        #region Add

        public override void Add(T myItem)
        {
            #region data

            var itemString = myItem.ToString();

            #endregion

            #region calc hash and insert

            foreach(var hash in _hashFunctions)
            {
                _BitArray.Set(Math.Abs(CalculateHash(hash, itemString)), true);
            }

            _NumberOAddedfItems++;

            #endregion
        }

        #endregion

        #region Contains

        public override bool Contains(T myItem)
        {
            #region data

            var itemString = myItem.ToString();

            #endregion

            #region calc hash and check if bit is set

            foreach(var hash in _hashFunctions)
            {
                if (!_BitArray[Math.Abs(CalculateHash(hash, itemString))])
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
