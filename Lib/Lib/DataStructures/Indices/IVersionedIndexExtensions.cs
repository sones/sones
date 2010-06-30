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


/*
 * IVersionedIndexExtensions
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// Extensions to the IVersionedIndexInterface interface
    /// </summary>
    public static class IVersionedIndexExtensions
    {


        #region ContainsKeys(myKeys, myVersion)

        public static Trinary ContainsKeys<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, IEnumerable<TKey> myKeys, Int64 myVersion)
            where TKey : IComparable
        {

            var _Success = Trinary.TRUE;

            foreach (var _Key in myKeys)
                _Success &= myIVersionedIndexInterface.ContainsKey(_Key, myVersion);

            return _Success;

        }

        #endregion

        #region ContainsValues(myValues, myVersion)

        public static Trinary ContainsValues<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, IEnumerable<TValue> myValues, Int64 myVersion)
            where TKey : IComparable
        {

            var _Success = Trinary.TRUE;

            foreach (var _Value in myValues)
                _Success &= myIVersionedIndexInterface.ContainsValue(_Value, myVersion);

            return _Success;

        }

        #endregion

        #region Contains(myKey, myValues, myVersion)

        public static Trinary Contains<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myKey, IEnumerable<TValue> myValues, Int64 myVersion)
            where TKey : IComparable
        {
            lock (myIVersionedIndexInterface)
            {

                HashSet<TValue> _HashSet;

                var _Success = myIVersionedIndexInterface.TryGetValue(myKey, out _HashSet, myVersion);

                if (_Success)
                {

                    foreach (var _Value in myValues)
                        if (!_HashSet.Contains(_Value))
                            return Trinary.FALSE;

                    return Trinary.TRUE;

                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #region Contains(myKeyValuePair, myVersion)

        public static Trinary Contains<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, KeyValuePair<TKey, TValue> myKeyValuePair, Int64 myVersion)
            where TKey : IComparable
        {
            return myIVersionedIndexInterface.Contains(myKeyValuePair.Key, myKeyValuePair.Value, myVersion);
        }

        #endregion

        #region Contains(myKeyValuesPair, myVersion)

        public static Trinary Contains<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, Int64 myVersion)
            where TKey : IComparable
        {
            return myIVersionedIndexInterface.Contains(myKeyValuesPair.Key, myKeyValuesPair.Value, myVersion);
        }

        #endregion

        #region Contains(myDictionary, myVersion)

        public static Trinary Contains<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, Dictionary<TKey, TValue> myDictionary, Int64 myVersion)
            where TKey : IComparable
        {

            var _Success = Trinary.TRUE;

            foreach (var _KeyValuePair in myDictionary)
                _Success &= myIVersionedIndexInterface.Contains(_KeyValuePair, myVersion);

            return _Success;

        }

        #endregion

        #region Contains(myMultiValueDictionary, myVersion)

        public static Trinary Contains<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, Int64 myVersion)
            where TKey : IComparable
        {

            var _Success = Trinary.TRUE;

            foreach (var _KeyValuesPair in myMultiValueDictionary)
                _Success &= myIVersionedIndexInterface.Contains(_KeyValuesPair, myVersion);

            return _Success;

        }

        #endregion


        #region Keys(myMinKey, myMaxKey, myVersion)

        public static IEnumerable<TKey> Keys<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myMinKey, TKey myMaxKey, Int64 myVersion)
            where TKey : IComparable
        {
            var lala = myIVersionedIndexInterface.Keys(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0, myVersion);
            return lala;
        }

        #endregion

        #region KeyCount(myMinKey, myMaxKey, myVersion)

        public static UInt64 KeyCount<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myMinKey, TKey myMaxKey, Int64 myVersion)
            where TKey : IComparable
        {
            return myIVersionedIndexInterface.KeyCount(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0, myVersion);
        }

        #endregion

        #region Values(myMinKey, myMaxKey, myVersion)

        public static IEnumerable<HashSet<TValue>> Values<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myMinKey, TKey myMaxKey, Int64 myVersion)
            where TKey : IComparable
        {
            var lala = myIVersionedIndexInterface.Values(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0, myVersion);
            return lala;
        }

        #endregion

        #region ValueCount(myMinKey, myMaxKey, myVersion)

        public static UInt64 ValueCount<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myMinKey, TKey myMaxKey, Int64 myVersion)
            where TKey : IComparable
        {
            return myIVersionedIndexInterface.ValueCount(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0, myVersion);
        }

        #endregion

        #region GetIDictionary(myMinKey, myMaxKey, myVersion)

        public static IDictionary<TKey, HashSet<TValue>> GetIDictionary<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myMinKey, TKey myMaxKey, Int64 myVersion)
            where TKey : IComparable
        {
            var lala = myIVersionedIndexInterface.GetIDictionary(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
            return lala;
        }

        #endregion

        #region GetEnumerator(myMinKey, myMaxKey, myVersion)

        public static IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator<TKey, TValue>(this IVersionedIndexInterface<TKey, TValue> myIVersionedIndexInterface, TKey myMinKey, TKey myMaxKey, Int64 myVersion)
            where TKey : IComparable
        {
            return myIVersionedIndexInterface.GetEnumerator(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0, myVersion);
        }

        #endregion


    }

}