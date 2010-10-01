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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Lib.Hashing.ConsistentHashing
{
    public class DictionaryCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _InternalDictionary;

        public DictionaryCache()
        {
            _InternalDictionary = new Dictionary<TKey, TValue>();
        }

        #region ICache Members

        public void AddItem(TKey key, TValue value)
        {
            if (!_InternalDictionary.ContainsKey(key))
            {
                _InternalDictionary.Add(key, value);
            }
        }

        public void RemoveItem(TKey key)
        {
            _InternalDictionary.Remove(key);
        }

        public TValue GetValue(TKey key)
        {
            TValue retVal = default(TValue);

            _InternalDictionary.TryGetValue(key, out retVal);

            return retVal;
        }        

        public String ItemName
        {
            get { return System.Convert.ToString(this.GetHashCode()); }
        }

        public TValue GetItem(TKey key)
        {
            return GetValue(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _InternalDictionary.GetEnumerator();
        }

        public void RemoveAllItems()
        {
            _InternalDictionary.Clear();
        }

        public Int64 Count()
        {
            return _InternalDictionary.Count();
        }

        #endregion
    }
}
