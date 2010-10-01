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

/* 
 * IIndexExtensions
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// Extensions to the IIndexInterface
    /// </summary>
    public static class IIndexExtensions
    {

        #region ContainsKeys(myKeys)

        public static Trinary ContainsKeys<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, IEnumerable<TKey> myKeys)
            where TKey : IComparable
        {

            var _Success = Trinary.TRUE;

            foreach (var _Key in myKeys)
                _Success &= myIIndexInterface.ContainsKey(_Key);

            return _Success;

        }

        #endregion

        #region ContainsValues(myValues)

        public static Trinary ContainsValues<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, IEnumerable<TValue> myValues)
            where TKey : IComparable
        {
            lock (myIIndexInterface)
            {

                var _Success = Trinary.TRUE;

                foreach (var _Value in myValues)
                    _Success &= myIIndexInterface.ContainsValue(_Value);

                return _Success;

            }
        }

        #endregion

        #region Contains(myKey, myValues)

        public static Trinary Contains<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myKey, IEnumerable<TValue> myValues)
            where TKey : IComparable
        {
            lock (myIIndexInterface)
            {

                HashSet<TValue> _HashSet;

                var _Success = myIIndexInterface.TryGetValue(myKey, out _HashSet);

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

        #region Contains(myKeyValuePair)

        public static Trinary Contains<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, KeyValuePair<TKey, TValue> myKeyValuePair)
            where TKey : IComparable
        {
            return myIIndexInterface.Contains(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Contains(myKeyValuesPair)

        public static Trinary Contains<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
            where TKey : IComparable
        {
            return myIIndexInterface.Contains(myKeyValuesPair.Key, myKeyValuesPair.Value);
        }

        #endregion

        #region Contains(myDictionary)

        public static Trinary Contains<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, Dictionary<TKey, TValue> myDictionary)
            where TKey : IComparable
        {

            lock (myIIndexInterface)
            {

                var _Success = true;

                foreach (var _KeyValuePair in myDictionary)
                    _Success &= myIIndexInterface.Contains(_KeyValuePair);

                return _Success;

            }

        }

        #endregion

        #region Contains(myMultiValueDictionary)

        public static Trinary Contains<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary)
            where TKey : IComparable
        {

            lock (myIIndexInterface)
            {

                var _Success = true;

                foreach (var _KeyValuesPair in myMultiValueDictionary)
                    _Success &= myIIndexInterface.Contains(_KeyValuesPair);

                return _Success;

            }

        }

        #endregion


        #region Keys(myMinKey, myMaxKey)

        public static IEnumerable<TKey> Keys<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIIndexInterface.Keys(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region KeyCount(myMinKey, myMaxKey)

        public static UInt64 KeyCount<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIIndexInterface.KeyCount(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region Values(myMinKey, myMaxKey)

        public static IEnumerable<HashSet<TValue>> Values<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            var lala = myIIndexInterface.Values(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
            return lala;
        }

        #endregion

        #region ValueCount(myMinKey, myMaxKey)

        public static UInt64 ValueCount<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIIndexInterface.ValueCount(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region GetIDictionary(myMinKey, myMaxKey)

        public static IDictionary<TKey, HashSet<TValue>> GetIDictionary<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIIndexInterface.GetIDictionary(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region GetEnumerator(myMinKey, myMaxKey)

        public static IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIIndexInterface.GetEnumerator(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion


        #region Remove(myKeys)

        public static Boolean Remove<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, IEnumerable<TKey> myKeys)
            where TKey : IComparable
        {

            lock (myIIndexInterface)
            {

                var _Success = true;

                foreach (var _Key in myKeys)
                    _Success &= myIIndexInterface.Remove(_Key);

                return _Success;

            }

        }

        #endregion

        #region Remove(myKeyValuePair)

        public static Boolean Remove<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, KeyValuePair<TKey, TValue> myKeyValuePair)
            where TKey : IComparable
        {
            return myIIndexInterface.Remove(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Remove(myDictionary)

        public static Boolean Remove<TKey, TValue>(this IIndexInterface<TKey, TValue> myIIndexInterface, Dictionary<TKey, TValue> myDictionary)
            where TKey : IComparable
        {

            lock (myIIndexInterface)
            {

                var _Success = true;

                foreach (var _KeyValuePair in myDictionary)
                    _Success &= myIIndexInterface.Remove(_KeyValuePair.Key, _KeyValuePair.Value);

                return _Success;

            }

        }

        #endregion

    }

}
