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
 * IDictionaryExtensions
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.Lib;

using sones.Lib.DataStructures;

#endregion

namespace sones.Lib.DataStructures.Dictionaries
{

    /// <summary>
    /// Extensions to the IDictionaryInterface interface
    /// </summary>

    public static class IDictionaryExtensions
    {

        #region ContainsKeys(myKeys)

        public static Trinary ContainsKeys<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, IEnumerable<TKey> myKeys)
            where TKey : IComparable
        {

            var _Success = Trinary.TRUE;

            foreach (var _Key in myKeys)
                _Success &= myIDictionaryInterface.ContainsKey(_Key);

            return _Success;

        }

        #endregion

        #region ContainsValues(myValues)

        public static Trinary ContainsValues<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, IEnumerable<TValue> myValues)
            where TKey : IComparable
        {

            lock (myIDictionaryInterface)
            {

                var _Success = Trinary.TRUE;

                foreach (var _Value in myValues)
                    _Success &= myIDictionaryInterface.ContainsValue(_Value);

                return _Success;

            }

        }

        #endregion

        #region ContainsValues(myKeyValuePair)

        public static Trinary Contains<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, KeyValuePair<TKey, TValue> myKeyValuePair)
            where TKey : IComparable
        {
            return myIDictionaryInterface.Contains(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region ContainsValues(myDictionary)

        public static Trinary Contains<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, Dictionary<TKey, TValue> myDictionary)
            where TKey : IComparable
        {

            lock (myIDictionaryInterface)
            {

                var _Success = true;

                foreach (var _KeyValuePair in myDictionary)
                    _Success &= myIDictionaryInterface.Contains(_KeyValuePair);

                return _Success;

            }

        }

        #endregion


        #region Keys(myMinKey, myMaxKey)

        public static IEnumerable<TKey> Keys<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIDictionaryInterface.Keys(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region KeyCount(myMinKey, myMaxKey)

        public static UInt64 KeyCount<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIDictionaryInterface.KeyCount(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region Values(myMinKey, myMaxKey)

        public static IEnumerable<TValue> Values<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIDictionaryInterface.Values(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region ValueCount(myMinKey, myMaxKey)

        public static UInt64 ValueCount<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIDictionaryInterface.ValueCount(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region GetIDictionary(myMinKey, myMaxKey)

        public static IDictionary<TKey, TValue> GetIDictionary<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIDictionaryInterface.GetIDictionary(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion

        #region GetEnumerator(myMinKey, myMaxKey)

        public static IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, TKey myMinKey, TKey myMaxKey)
            where TKey : IComparable
        {
            return myIDictionaryInterface.GetEnumerator(item => item.Key.CompareTo(myMinKey) >= 0 && item.Key.CompareTo(myMaxKey) <= 0);
        }

        #endregion


        #region Remove(myKeyValuePair)

        public static Boolean Remove<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, IEnumerable<TKey> myKeys)
            where TKey : IComparable
        {

            lock (myIDictionaryInterface)
            {

                var _Success = true;

                foreach (var _Key in myKeys)
                    _Success &= myIDictionaryInterface.Remove(_Key);

                return _Success;

            }

        }

        #endregion

        #region Remove(myKeyValuePair)

        public static Boolean Remove<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, KeyValuePair<TKey, TValue> myKeyValuePair)
            where TKey : IComparable
        {
            return myIDictionaryInterface.Remove(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Remove(myDictionary)

        public static Boolean Remove<TKey, TValue>(this IDictionaryInterface<TKey, TValue> myIDictionaryInterface, Dictionary<TKey, TValue> myDictionary)
            where TKey : IComparable
        {

            lock (myIDictionaryInterface)
            {

                var _Success = true;

                foreach (var _KeyValuePair in myDictionary)
                    _Success &= myIDictionaryInterface.Remove(_KeyValuePair.Key, _KeyValuePair.Value);

                return _Success;

            }

        }

        #endregion

    }

}